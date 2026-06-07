using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    // Download task information
    public class DownloadTask
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; }
        public string Url { get; set; }
        public string LocalPath { get; set; }
        public string ExpectedSha1 { get; set; }
        public long TotalBytes { get; set; }
        public long DownloadedBytes { get; set; }
        public float Speed { get; set; }
        public DownloadState State { get; set; } = DownloadState.Pending;
        public int Priority { get; set; } = 0;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ErrorMessage { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public TimeSpan EstimatedTimeRemaining { get; set; }
        public int Progress => TotalBytes > 0 ? (int)((float)DownloadedBytes / TotalBytes * 100) : 0;

        public event Action<DownloadTask> ProgressChanged;
        public event Action<DownloadTask> StateChanged;

        public void UpdateProgress(long downloaded, float speed)
        {
            DownloadedBytes = downloaded;
            Speed = speed;

            if (speed > 0 && TotalBytes > downloaded)
            {
                long remaining = TotalBytes - downloaded;
                EstimatedTimeRemaining = TimeSpan.FromSeconds(remaining / speed);
            }

            ProgressChanged?.Invoke(this);
        }

        public void SetState(DownloadState newState)
        {
            State = newState;
            if (newState == DownloadState.Completed || newState == DownloadState.Failed || newState == DownloadState.Cancelled)
            {
                EndTime = DateTime.Now;
            }
            StateChanged?.Invoke(this);
        }
    }

    // Download manager with multi-threading support
    public class DownloadManager : IDisposable
    {
        private HttpClient httpClient;
        private ConcurrentQueue<DownloadTask> downloadQueue;
        private List<DownloadTask> activeDownloads;
        private List<DownloadTask> completedDownloads;
        private SemaphoreSlim downloadSemaphore;
        private CancellationTokenSource managerCts;
        private bool isDisposed = false;
        private int maxConcurrentDownloads = 4;
        private long maxBytesPerSecond = 0; // 0 = unlimited
        private Timer speedCalculationTimer;
        private Dictionary<string, long> lastBytesDownloaded;
        private Dictionary<string, DateTime> lastUpdateTime;

        public event Action<DownloadTask> TaskAdded;
        public event Action<DownloadTask> TaskProgressChanged;
        public event Action<DownloadTask> TaskStateChanged;
        public event Action<DownloadTask> TaskCompleted;
        public event Action<DownloadTask> TaskFailed;
        public event Action AllTasksCompleted;

        public int MaxConcurrentDownloads
        {
            get { return maxConcurrentDownloads; }
            set { maxConcurrentDownloads = Math.Max(1, Math.Min(16, value)); }
        }

        public long MaxBytesPerSecond
        {
            get { return maxBytesPerSecond; }
            set { maxBytesPerSecond = Math.Max(0, value); }
        }

        public int ActiveCount => activeDownloads.Count;
        public int QueuedCount => downloadQueue.Count;
        public int CompletedCount => completedDownloads.Count;

        public DownloadManager()
        {
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(30);
            downloadQueue = new ConcurrentQueue<DownloadTask>();
            activeDownloads = new List<DownloadTask>();
            completedDownloads = new List<DownloadTask>();
            downloadSemaphore = new SemaphoreSlim(maxConcurrentDownloads, maxConcurrentDownloads);
            managerCts = new CancellationTokenSource();
            lastBytesDownloaded = new Dictionary<string, long>();
            lastUpdateTime = new Dictionary<string, DateTime>();

            speedCalculationTimer = new Timer(CalculateSpeeds, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        public DownloadTask AddDownload(string url, string localPath, string expectedSha1 = null, int priority = 0)
        {
            var task = new DownloadTask
            {
                Url = url,
                FileName = Path.GetFileName(localPath),
                LocalPath = localPath,
                ExpectedSha1 = expectedSha1,
                Priority = priority,
                CancellationTokenSource = new CancellationTokenSource(),
                StartTime = DateTime.Now
            };

            task.ProgressChanged += t => TaskProgressChanged?.Invoke(t);
            task.StateChanged += t =>
            {
                TaskStateChanged?.Invoke(t);
                if (t.State == DownloadState.Completed)
                    TaskCompleted?.Invoke(t);
                else if (t.State == DownloadState.Failed)
                    TaskFailed?.Invoke(t);
            };

            downloadQueue.Enqueue(task);
            TaskAdded?.Invoke(task);

            ProcessQueue();

            return task;
        }

        public void AddDownloads(IEnumerable<DownloadItemInfo> items)
        {
            foreach (var item in items)
            {
                AddDownload(item.Url, item.LocalPath, item.ExpectedSha1, item.Priority);
            }
        }

        public class DownloadItemInfo
        {
            public string Url { get; set; }
            public string LocalPath { get; set; }
            public string ExpectedSha1 { get; set; }
            public int Priority { get; set; }
        }

        private async void ProcessQueue()
        {
            while (!managerCts.IsCancellationRequested && downloadQueue.TryDequeue(out var task))
            {
                await downloadSemaphore.WaitAsync(managerCts.Token);

                lock (activeDownloads)
                {
                    activeDownloads.Add(task);
                }

                task.SetState(DownloadState.Initializing);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await DownloadFileAsync(task);
                    }
                    catch (Exception ex)
                    {
                        task.ErrorMessage = ex.Message;
                        task.SetState(DownloadState.Failed);
                    }
                    finally
                    {
                        lock (activeDownloads)
                        {
                            activeDownloads.Remove(task);
                        }

                        if (task.State == DownloadState.Completed)
                        {
                            lock (completedDownloads)
                            {
                                completedDownloads.Add(task);
                            }
                        }

                        downloadSemaphore.Release();
                        ProcessQueue();
                    }
                }, managerCts.Token);
            }

            if (activeDownloads.Count == 0 && downloadQueue.Count == 0)
            {
                AllTasksCompleted?.Invoke();
            }
        }

        private async Task DownloadFileAsync(DownloadTask task)
        {
            task.SetState(DownloadState.Downloading);

            string directory = Path.GetDirectoryName(task.LocalPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Check if file already exists with correct hash
            if (File.Exists(task.LocalPath) && !string.IsNullOrEmpty(task.ExpectedSha1))
            {
                string existingHash = ComputeSha1(task.LocalPath);
                if (existingHash.Equals(task.ExpectedSha1, StringComparison.OrdinalIgnoreCase))
                {
                    task.DownloadedBytes = task.TotalBytes = new FileInfo(task.LocalPath).Length;
                    task.SetState(DownloadState.Completed);
                    return;
                }
            }

            using (var response = await httpClient.GetAsync(task.Url, HttpCompletionOption.ResponseHeadersRead, task.CancellationTokenSource.Token))
            {
                response.EnsureSuccessStatusCode();
                task.TotalBytes = response.Content.Headers.ContentLength ?? 0;

                var tempPath = task.LocalPath + ".tmp";

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    var buffer = new byte[8192];
                    int bytesRead;
                    long totalRead = 0;
                    var stopwatch = Stopwatch.StartNew();

                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, task.CancellationTokenSource.Token)) > 0)
                    {
                        if (task.State == DownloadState.Paused)
                        {
                            await WaitForResumeAsync(task);
                        }

                        if (task.CancellationTokenSource.Token.IsCancellationRequested)
                        {
                            task.SetState(DownloadState.Cancelled);
                            return;
                        }

                        await fileStream.WriteAsync(buffer, 0, bytesRead, task.CancellationTokenSource.Token);
                        totalRead += bytesRead;

                        // Bandwidth throttling
                        if (maxBytesPerSecond > 0)
                        {
                            long expectedBytes = (long)(stopwatch.Elapsed.TotalSeconds * maxBytesPerSecond);
                            if (totalRead > expectedBytes)
                            {
                                int delayMs = (int)((totalRead - expectedBytes) * 1000.0 / maxBytesPerSecond);
                                if (delayMs > 0)
                                    await Task.Delay(delayMs, task.CancellationTokenSource.Token);
                            }
                        }

                        // Update progress
                        lock (lastBytesDownloaded)
                        {
                            lastBytesDownloaded[task.Id] = totalRead;
                            lastUpdateTime[task.Id] = DateTime.Now;
                        }

                        task.UpdateProgress(totalRead, task.Speed);
                    }
                }

                // Verify hash
                if (!string.IsNullOrEmpty(task.ExpectedSha1))
                {
                    string actualHash = ComputeSha1(tempPath);
                    if (!actualHash.Equals(task.ExpectedSha1, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Delete(tempPath);
                        task.ErrorMessage = "SHA1 verification failed";
                        task.SetState(DownloadState.Failed);
                        return;
                    }
                }

                // Move temp file to final location
                if (File.Exists(task.LocalPath))
                    File.Delete(task.LocalPath);
                File.Move(tempPath, task.LocalPath);

                task.SetState(DownloadState.Completed);
            }
        }

        private async Task WaitForResumeAsync(DownloadTask task)
        {
            while (task.State == DownloadState.Paused && !task.CancellationTokenSource.Token.IsCancellationRequested)
            {
                await Task.Delay(100, task.CancellationTokenSource.Token);
            }
        }

        private void CalculateSpeeds(object state)
        {
            lock (lastBytesDownloaded)
            {
                foreach (var task in activeDownloads)
                {
                    if (lastBytesDownloaded.TryGetValue(task.Id, out long lastBytes) &&
                        lastUpdateTime.TryGetValue(task.Id, out DateTime lastTime))
                    {
                        double elapsed = (DateTime.Now - lastTime).TotalSeconds;
                        if (elapsed > 0)
                        {
                            long bytesDiff = task.DownloadedBytes - lastBytes;
                            float speed = (float)(bytesDiff / elapsed);
                            task.UpdateProgress(task.DownloadedBytes, speed);
                        }
                    }

                    lastBytesDownloaded[task.Id] = task.DownloadedBytes;
                    lastUpdateTime[task.Id] = DateTime.Now;
                }
            }
        }

        public void PauseTask(DownloadTask task)
        {
            if (task.State == DownloadState.Downloading)
            {
                task.SetState(DownloadState.Paused);
            }
        }

        public void ResumeTask(DownloadTask task)
        {
            if (task.State == DownloadState.Paused)
            {
                task.SetState(DownloadState.Downloading);
            }
        }

        public void CancelTask(DownloadTask task)
        {
            task.CancellationTokenSource?.Cancel();
            task.SetState(DownloadState.Cancelled);
        }

        public void RetryTask(DownloadTask task)
        {
            if (task.State == DownloadState.Failed || task.State == DownloadState.Cancelled)
            {
                task.CancellationTokenSource = new CancellationTokenSource();
                task.DownloadedBytes = 0;
                task.ErrorMessage = null;
                task.SetState(DownloadState.Pending);
                downloadQueue.Enqueue(task);
                ProcessQueue();
            }
        }

        public void CancelAll()
        {
            managerCts.Cancel();

            foreach (var task in activeDownloads)
            {
                task.CancellationTokenSource?.Cancel();
            }
        }

        public List<DownloadTask> GetActiveDownloads()
        {
            lock (activeDownloads)
            {
                return activeDownloads.ToList();
            }
        }

        public List<DownloadTask> GetCompletedDownloads()
        {
            lock (completedDownloads)
            {
                return completedDownloads.ToList();
            }
        }

        public List<DownloadTask> GetQueuedDownloads()
        {
            return downloadQueue.ToList();
        }

        public void SaveState(string filePath)
        {
            var state = new
            {
                ActiveDownloads = activeDownloads.Select(t => new
                {
                    t.Id, t.Url, t.LocalPath, t.ExpectedSha1, t.Priority,
                    t.DownloadedBytes, t.TotalBytes, t.State
                }),
                CompletedDownloads = completedDownloads.Select(t => new
                {
                    t.Id, t.Url, t.LocalPath, t.ExpectedSha1, t.Priority
                })
            };

            string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public void LoadState(string filePath)
        {
            if (!File.Exists(filePath)) return;

            try
            {
                string json = File.ReadAllText(filePath);
                var state = JsonSerializer.Deserialize<JsonElement>(json);

                // Load completed downloads
                if (state.TryGetProperty("CompletedDownloads", out var completed))
                {
                    foreach (var item in completed.EnumerateArray())
                    {
                        var task = new DownloadTask
                        {
                            Id = item.GetProperty("Id").GetString(),
                            Url = item.GetProperty("Url").GetString(),
                            LocalPath = item.GetProperty("LocalPath").GetString(),
                            ExpectedSha1 = item.TryGetProperty("ExpectedSha1", out var sha1) ? sha1.GetString() : null,
                            State = DownloadState.Completed
                        };
                        completedDownloads.Add(task);
                    }
                }
            }
            catch { }
        }

        private static string ComputeSha1(string filePath)
        {
            using (var sha1 = SHA1.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = sha1.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                managerCts?.Cancel();
                speedCalculationTimer?.Dispose();
                httpClient?.Dispose();
                downloadSemaphore?.Dispose();

                foreach (var task in activeDownloads)
                {
                    task.CancellationTokenSource?.Dispose();
                }
            }
        }
    }
}
