﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        private StartMinecraft launcher;
        private DownloadManager downloadManager;
        private List<VersionInfo> allVersions;
        private VersionInfo selectedVersion;
        private string currentFilter = "all";
        private Dictionary<string, DownloadItem> downloadControls;
        private Settings settings;

        public Form1()
        {
            InitializeComponent();
            launcher = new StartMinecraft();
            downloadManager = new DownloadManager();
            allVersions = new List<VersionInfo>();
            downloadControls = new Dictionary<string, DownloadItem>();
            settings = Settings.Load();

            SetupDownloadManagerEvents();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtGameDir.Text = launcher.GameDir;
            txtJavaPath.Text = settings.JavaPath;
            cmbMemory.SelectedIndex = GetMemoryIndex(settings.MemoryGB);
            chkAutoUpdate.Checked = settings.AutoUpdate;
            chkKeepLauncherOpen.Checked = settings.KeepLauncherOpen;

            if (settings.AutoUpdate)
            {
                _ = LoadVersionsAsync();
            }
        }

        private int GetMemoryIndex(int gb)
        {
            int[] values = { 1, 2, 4, 6, 8, 12, 16 };
            int index = Array.IndexOf(values, gb);
            return index >= 0 ? index : 1;
        }

        private int GetMemoryFromIndex(int index)
        {
            int[] values = { 1, 2, 4, 6, 8, 12, 16 };
            return values[index];
        }

        private void SetupDownloadManagerEvents()
        {
            downloadManager.TaskAdded += task => Invoke(new Action(() => AddDownloadControl(task)));
            downloadManager.TaskProgressChanged += task => Invoke(new Action(() => UpdateDownloadControl(task)));
            downloadManager.TaskStateChanged += task => Invoke(new Action(() => UpdateDownloadControlState(task)));
            downloadManager.TaskCompleted += task => Invoke(new Action(() => OnDownloadCompleted(task)));
            downloadManager.TaskFailed += task => Invoke(new Action(() => OnDownloadFailed(task)));
            downloadManager.AllTasksCompleted += () => Invoke(new Action(() => OnAllDownloadsCompleted()));
        }

        #region Version Management

        private async Task LoadVersionsAsync()
        {
            try
            {
                lblVersionTitle.Text = "加载中...";
                txtVersionInfo.Text = "正在从 BMCLAPI 获取版本列表...";

                string manifest = await launcher.GetVersionManifestAsync();
                var root = JsonNode.Parse(manifest);
                var versions = root?["versions"]?.AsArray();

                if (versions == null)
                {
                    txtVersionInfo.Text = "版本列表解析失败";
                    return;
                }

                allVersions.Clear();

                foreach (var v in versions)
                {
                    string id = v?["id"]?.GetValue<string>();
                    string type = v?["type"]?.GetValue<string>();
                    string url = v?["url"]?.GetValue<string>();
                    string time = v?["releaseTime"]?.GetValue<string>() ?? "";

                    if (string.IsNullOrEmpty(id)) continue;

                    allVersions.Add(new VersionInfo
                    {
                        Id = id,
                        Type = type,
                        Url = url,
                        ReleaseTime = time
                    });
                }

                ApplyVersionFilter(currentFilter);
                lblVersionTitle.Text = string.Format("已加载 {0} 个版本", allVersions.Count);
            }
            catch (Exception ex)
            {
                txtVersionInfo.Text = "加载版本失败: " + ex.Message;
            }
        }

        private void ApplyVersionFilter(string filter)
        {
            currentFilter = filter;
            panelVersionList.Controls.Clear();

            IEnumerable<VersionInfo> filtered = allVersions;

            switch (filter.ToLower())
            {
                case "release":
                    filtered = allVersions.Where(v => v.Type.Equals("release", StringComparison.OrdinalIgnoreCase));
                    break;
                case "snapshot":
                    filtered = allVersions.Where(v => v.Type.Equals("snapshot", StringComparison.OrdinalIgnoreCase));
                    break;
                case "april_fool":
                case "april fool":
                    filtered = allVersions.Where(v =>
                        v.Type.Equals("april_fool", StringComparison.OrdinalIgnoreCase) ||
                        v.Id.IndexOf("20w14infinite", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        v.Id.IndexOf("15w14a", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        v.Id.IndexOf("3D Shareware", StringComparison.OrdinalIgnoreCase) >= 0);
                    break;
            }

            foreach (var version in filtered.Take(50))
            {
                var card = CreateVersionCard(version);
                panelVersionList.Controls.Add(card);
            }
        }

        private VersionCard CreateVersionCard(VersionInfo version)
        {
            var card = new VersionCard
            {
                VersionId = version.Id,
                VersionType = version.Type,
                VersionDate = FormatReleaseTime(version.ReleaseTime),
                Width = panelVersionList.Width - 20,
                Margin = new Padding(5)
            };

            card.CardSelected += (s, e) => SelectVersion(version);

            return card;
        }

        private string FormatReleaseTime(string time)
        {
            if (string.IsNullOrEmpty(time)) return "";
            try
            {
                var date = DateTime.Parse(time);
                return date.ToString("yyyy-MM-dd");
            }
            catch { return ""; }
        }

        private void SelectVersion(VersionInfo version)
        {
            selectedVersion = version;

            // Update UI
            foreach (VersionCard card in panelVersionList.Controls)
            {
                card.IsSelected = (card.VersionId == version.Id);
            }

            // Ensure detail panel and controls are visible
            panelVersionInfo.Visible = true;
            panelVersionInfo.BringToFront();
            lblVersionTitle.Visible = true;
            lblVersionType.Visible = true;
            txtVersionInfo.Visible = true;
            btnDownloadVersion.Visible = true;
            btnLaunchVersion.Visible = true;

            lblVersionTitle.Text = version.Id;
            lblVersionType.Text = version.Type.ToUpper();

            // Apply color based on version type
            Color themeColor;
            switch (version.Type.ToLower())
            {
                case "snapshot":
                    themeColor = ColorSchemes.Snapshot.Primary;
                    break;
                case "april_fool":
                    themeColor = ColorSchemes.AprilFool.Primary;
                    break;
                default:
                    themeColor = ColorSchemes.Release.Primary;
                    break;
            }

            headerPanel.BackColor = themeColor;
            btnDownloadVersion.BackColor = ColorSchemes.Info;
            btnLaunchVersion.BackColor = ColorSchemes.Success;
            btnDownloadVersion.ForeColor = Color.White;
            btnLaunchVersion.ForeColor = Color.White;

            // Show version info
            txtVersionInfo.Text = GetVersionDescription(version);

            // Show launch settings
            panelLaunchSettings.Visible = true;
        }

        private string GetVersionDescription(VersionInfo version)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Version: " + version.Id);
            sb.AppendLine("Type: " + version.Type.ToUpper());
            sb.AppendLine("Released: " + FormatReleaseTime(version.ReleaseTime));
            sb.AppendLine();

            // Add version-specific descriptions
            if (version.Type.Equals("release", StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine("This is an official stable release.");
                sb.AppendLine();
                sb.AppendLine("Features:");
                sb.AppendLine("- Stable gameplay experience");
                sb.AppendLine("- Recommended for most players");
                sb.AppendLine("- Full mod compatibility");
            }
            else if (version.Type.Equals("snapshot", StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine("This is a development snapshot.");
                sb.AppendLine();
                sb.AppendLine("Note:");
                sb.AppendLine("- May contain bugs");
                sb.AppendLine("- Features are subject to change");
                sb.AppendLine("- Not recommended for production worlds");
            }
            else if (version.Type.Equals("april_fool", StringComparison.OrdinalIgnoreCase) ||
                     version.Id.Contains("20w14infinite") ||
                     version.Id.Contains("15w14a"))
            {
                sb.AppendLine("This is an April Fool's version!");
                sb.AppendLine();
                sb.AppendLine("Fun features:");
                sb.AppendLine("- Special joke content");
                sb.AppendLine("- Unique gameplay mechanics");
                sb.AppendLine("- Limited time features");
            }

            return sb.ToString();
        }

        #endregion

        #region Button Events

        private async void btnFilter_Click(object sender, EventArgs e)
        {
            if (allVersions.Count == 0)
            {
                await LoadVersionsAsync();
                return;
            }

            string filter = "all";
            if (sender == btnFilterRelease) filter = "release";
            else if (sender == btnFilterSnapshot) filter = "snapshot";
            else if (sender == btnFilterAprilFool) filter = "april_fool";

            ApplyVersionFilter(filter);
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            if (selectedVersion == null)
            {
                MessageBox.Show("Please select a version first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            launcher.GameDir = txtGameDir.Text;

            try
            {
                btnDownloadVersion.Enabled = false;
                btnDownloadVersion.Text = "下载中...";

                // Switch to downloads tab
                tabMain.SelectedTab = tabDownloads;

                // Start download using download manager
                await DownloadVersionAsync(selectedVersion);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Download failed: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDownloadVersion.Enabled = true;
                btnDownloadVersion.Text = "下载";
            }
        }

        private async Task DownloadVersionAsync(VersionInfo version)
        {
            // Download version JSON
            string versionJson = await launcher.DownloadVersionJson(version.Id);
            if (string.IsNullOrEmpty(versionJson))
            {
                throw new Exception("Failed to download version JSON");
            }

            var root = JsonNode.Parse(versionJson);

            // Queue client jar download
            var clientNode = root?["downloads"]?["client"];
            if (clientNode != null)
            {
                string clientUrl = clientNode["url"]?.GetValue<string>();
                string clientSha1 = clientNode["sha1"]?.GetValue<string>();
                string clientPath = Path.Combine(launcher.GameDir, "versions", version.Id, version.Id + ".jar");

                downloadManager.AddDownload(clientUrl, clientPath, clientSha1, 10);
            }

            // Queue library downloads
            var libraries = root?["libraries"]?.AsArray();
            if (libraries != null)
            {
                string osName = GetOsName();
                foreach (var lib in libraries)
                {
                    if (!IsLibraryAllowed(lib, osName)) continue;

                    var artifact = lib?["downloads"]?["artifact"];
                    if (artifact == null) continue;

                    string libUrl = artifact["url"]?.GetValue<string>();
                    string libPath = artifact["path"]?.GetValue<string>();
                    string libSha1 = artifact["sha1"]?.GetValue<string>();

                    if (!string.IsNullOrEmpty(libUrl) && !string.IsNullOrEmpty(libPath))
                    {
                        string fullPath = Path.Combine(launcher.GameDir, "libraries", libPath);
                        downloadManager.AddDownload(libUrl, fullPath, libSha1, 5);
                    }
                }
            }

            // Queue asset downloads
            var assetIndex = root?["assetIndex"];
            if (assetIndex != null)
            {
                string assetUrl = assetIndex["url"]?.GetValue<string>();
                string assetId = assetIndex["id"]?.GetValue<string>();
                string assetSha1 = assetIndex["sha1"]?.GetValue<string>();
                string assetPath = Path.Combine(launcher.GameDir, "assets", "indexes", assetId + ".json");

                downloadManager.AddDownload(assetUrl, assetPath, assetSha1, 8);
            }
        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            if (selectedVersion == null)
            {
                MessageBox.Show("Please select a version first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = txtUsername.Text.Trim();
            if (string.IsNullOrEmpty(username)) username = "Player";

            launcher.GameDir = txtGameDir.Text;
            launcher.JavaPath = txtJavaPath.Text;

            try
            {
                var process = launcher.Launch(selectedVersion.Id, username, LogLaunchOutput);
                if (process != null)
                {
                    MessageBox.Show(string.Format("Minecraft started (PID {0})", process.Id),
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (!chkKeepLauncherOpen.Checked)
                    {
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Launch failed:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LogLaunchOutput(string message)
        {
            Debug.WriteLine("[MC] " + message);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select Minecraft game directory";
                dialog.SelectedPath = txtGameDir.Text;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtGameDir.Text = dialog.SelectedPath;
                    launcher.GameDir = dialog.SelectedPath;
                }
            }
        }

        private void btnBrowseJava_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Java Executable|javaw.exe|All Files|*.*";
                dialog.Title = "Select Java Executable";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtJavaPath.Text = dialog.FileName;
                }
            }
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            settings.JavaPath = txtJavaPath.Text;
            settings.MemoryGB = GetMemoryFromIndex(cmbMemory.SelectedIndex);
            settings.AutoUpdate = chkAutoUpdate.Checked;
            settings.KeepLauncherOpen = chkKeepLauncherOpen.Checked;
            settings.Save();

            MessageBox.Show("Settings saved!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Download Management

        private void AddDownloadControl(DownloadTask task)
        {
            if (downloadControls.ContainsKey(task.Id)) return;

            var control = new DownloadItem
            {
                FileName = task.FileName,
                Url = task.Url,
                Width = panelDownloadList.Width - 20,
                Margin = new Padding(5)
            };

            control.PauseRequested += (s, e) => downloadManager.PauseTask(task);
            control.ResumeRequested += (s, e) => downloadManager.ResumeTask(task);
            control.CancelRequested += (s, e) => downloadManager.CancelTask(task);
            control.RetryRequested += (s, e) => downloadManager.RetryTask(task);
            control.OpenLocationRequested += (s, e) => {
                if (File.Exists(task.LocalPath))
                    Process.Start("explorer.exe", "/select,\"" + task.LocalPath + "\"");
            };
            control.CopyUrlRequested += (s, e) => {
                if (!string.IsNullOrEmpty(task.Url))
                    Clipboard.SetText(task.Url);
            };

            downloadControls[task.Id] = control;
            panelDownloadList.Controls.Add(control);
            UpdateDownloadStats();
        }

        private void UpdateDownloadControl(DownloadTask task)
        {
            if (downloadControls.TryGetValue(task.Id, out var control))
            {
                control.TotalBytes = task.TotalBytes;
                control.DownloadedBytes = task.DownloadedBytes;
                control.Speed = task.Speed;
            }
            UpdateDownloadStats();
        }

        private void UpdateDownloadControlState(DownloadTask task)
        {
            if (downloadControls.TryGetValue(task.Id, out var control))
            {
                control.State = task.State;
            }
            UpdateDownloadStats();
        }

        private void OnDownloadCompleted(DownloadTask task)
        {
            UpdateDownloadStats();
        }

        private void OnDownloadFailed(DownloadTask task)
        {
            UpdateDownloadStats();
        }

        private void OnAllDownloadsCompleted()
        {
            lblOverallStatus.Text = "全部下载完成！";
            overallProgress.Value = 100;
            overallProgress.CustomText = "完成";

            MessageBox.Show("所有文件下载成功！", "下载完成",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateDownloadStats()
        {
            int active = downloadManager.ActiveCount;
            int queued = downloadManager.QueuedCount;
            int completed = downloadManager.CompletedCount;

            lblActiveDownloads.Text = string.Format("Active: {0} | Queued: {1} | Completed: {2}",
                active, queued, completed);

            if (active > 0 || queued > 0)
            {
                lblOverallStatus.Text = string.Format("Downloading {0} file(s)...", active);
                overallProgress.CustomText = string.Format("{0} active, {1} queued", active, queued);
            }
            else
            {
                lblOverallStatus.Text = "暂无下载任务";
            }
        }

        private void cmbSpeedLimit_SelectedIndexChanged(object sender, EventArgs e)
        {
            long bytesPerSecond = 0;
            switch (cmbSpeedLimit.SelectedIndex)
            {
                case 1: bytesPerSecond = 1024 * 1024; break; // 1 MB/s
                case 2: bytesPerSecond = 2 * 1024 * 1024; break; // 2 MB/s
                case 3: bytesPerSecond = 5 * 1024 * 1024; break; // 5 MB/s
                case 4: bytesPerSecond = 10 * 1024 * 1024; break; // 10 MB/s
            }
            downloadManager.MaxBytesPerSecond = bytesPerSecond;
        }

        private void cmbConcurrentDownloads_SelectedIndexChanged(object sender, EventArgs e)
        {
            int count = int.Parse(cmbConcurrentDownloads.SelectedItem.ToString());
            downloadManager.MaxConcurrentDownloads = count;
        }

        #endregion

        #region Helper Methods

        private static string GetOsName()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix) return "linux";
            if (Environment.OSVersion.Platform == PlatformID.MacOSX) return "osx";
            return "windows";
        }

        private static bool IsLibraryAllowed(JsonNode lib, string osName)
        {
            var rules = lib?["rules"]?.AsArray();
            if (rules == null || rules.Count == 0) return true;

            bool allow = false;
            foreach (var rule in rules)
            {
                string action = rule?["action"]?.GetValue<string>();
                var os = rule?["os"];
                if (os == null)
                {
                    allow = (action == "allow");
                    continue;
                }
                string ruleOs = os["name"]?.GetValue<string>();
                if (ruleOs == osName)
                {
                    allow = (action == "allow");
                }
            }
            return allow;
        }

        #endregion
    }

    #region Data Classes

    public class VersionInfo
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string ReleaseTime { get; set; }
    }

    public class Settings
    {
        public string JavaPath { get; set; } = "";
        public int MemoryGB { get; set; } = 2;
        public bool AutoUpdate { get; set; } = true;
        public bool KeepLauncherOpen { get; set; } = false;

        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MinecraftLauncher", "settings.json");

        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(SettingsPath);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                string json = System.Text.Json.JsonSerializer.Serialize(this);
                File.WriteAllText(SettingsPath, json);
            }
            catch { }
        }

        public static Settings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    return System.Text.Json.JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
                }
            }
            catch { }
            return new Settings();
        }
    }

    #endregion
}
