using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    public class StartMinecraft
    {
        private HttpClient httpClient;

        public string[] javas;
        public string GameDir { get; set; }
        public string JavaPath { get; set; }

        public StartMinecraft()
        {
            httpClient = new HttpClient();
            GameDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".minecraft");
        }

        public void Init()
        {
            string s = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (s == null) return;
            javas = new string[] { s };
        }

        public async Task Download(string versionId)
        {
            string versionJson = await DownloadVersionJson(versionId);
            if (string.IsNullOrEmpty(versionJson)) return;

            await DownloadClientJar(versionJson);
            await DownloadLibraries(versionJson);
            await DownloadAssets(versionJson);
        }

        public Process Launch(string versionId, string username, Action<string> log = null)
        {
            string versionDir = Path.Combine(GameDir, "versions", versionId);
            string jsonPath = Path.Combine(versionDir, versionId + ".json");
            string jarPath = Path.Combine(versionDir, versionId + ".jar");

            if (!File.Exists(jsonPath)) throw new FileNotFoundException("Version JSON not found: " + jsonPath);
            if (!File.Exists(jarPath)) throw new FileNotFoundException("Client jar not found: " + jarPath);

            string json = File.ReadAllText(jsonPath);
            var root = JsonNode.Parse(json);
            if (root == null) throw new InvalidDataException("Failed to parse version.json");

            string mainClass = root["mainClass"]?.GetValue<string>();
            if (string.IsNullOrEmpty(mainClass)) throw new InvalidDataException("No mainClass in version.json");

            string assetIndexId = root["assetIndex"]?["id"]?.GetValue<string>() ?? versionId;
            string osName = GetOsName();

            string nativesDir = Path.Combine(versionDir, versionId + "-natives");
            Directory.CreateDirectory(nativesDir);

            var classpath = new List<string>();
            var libraries = root["libraries"]?.AsArray();
            if (libraries != null)
            {
                foreach (var lib in libraries)
                {
                    if (!IsLibraryAllowed(lib, osName)) continue;
                    var artifact = lib?["downloads"]?["artifact"];
                    if (artifact == null) continue;
                    string libRelPath = artifact["path"]?.GetValue<string>();
                    if (string.IsNullOrEmpty(libRelPath)) continue;
                    string libFullPath = Path.Combine(GameDir, "libraries", libRelPath);
                    if (File.Exists(libFullPath)) classpath.Add(libFullPath);
                }
            }
            classpath.Add(jarPath);

            string cp = string.Join(Path.PathSeparator.ToString(), classpath);
            string uuid = Guid.NewGuid().ToString("N");

            var args = new StringBuilder();
            args.Append("-XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow ");
            args.Append("-Xmx2G -Xms512M ");
            args.Append(string.Format("-Djava.library.path=\"{0}\" ", nativesDir));
            args.Append(string.Format("-Dminecraft.client.jar=\"{0}\" ", jarPath));
            args.Append(string.Format("-cp \"{0}\" ", cp));
            args.Append(string.Format("{0} ", mainClass));
            args.Append(string.Format("--username {0} ", username));
            args.Append(string.Format("--version \"{0}\" ", versionId));
            args.Append(string.Format("--gameDir \"{0}\" ", GameDir));
            args.Append(string.Format("--assetsDir \"{0}\" ", Path.Combine(GameDir, "assets")));
            args.Append(string.Format("--assetIndex {0} ", assetIndexId));
            args.Append(string.Format("--uuid {0} ", uuid));
            args.Append("--accessToken 0 --userType legacy --versionType release");

            string javaExe = ResolveJava();
            if (log != null) log("Java: " + javaExe);
            if (log != null) log("Main class: " + mainClass);
            if (log != null) log("Classpath entries: " + classpath.Count);

            var startInfo = new ProcessStartInfo
            {
                FileName = javaExe,
                Arguments = args.ToString(),
                WorkingDirectory = GameDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false
            };

            var process = Process.Start(startInfo);
            if (process != null && log != null)
            {
                process.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) log("[MC] " + e.Data); };
                process.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) log("[MC ERR] " + e.Data); };
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            return process;
        }

        private string ResolveJava()
        {
            if (!string.IsNullOrEmpty(JavaPath) && File.Exists(JavaPath)) return JavaPath;

            string javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(javaHome))
            {
                string javaw = Path.Combine(javaHome, "bin", "javaw.exe");
                if (File.Exists(javaw)) return javaw;
                string javaExe = Path.Combine(javaHome, "bin", "java.exe");
                if (File.Exists(javaExe)) return javaExe;
            }

            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            foreach (string baseDir in new[] { programFiles, programFilesX86 })
            {
                string javaDir = Path.Combine(baseDir, "Java");
                if (!Directory.Exists(javaDir)) continue;
                foreach (string dir in Directory.GetDirectories(javaDir))
                {
                    string javaw = Path.Combine(dir, "bin", "javaw.exe");
                    if (File.Exists(javaw)) return javaw;
                }
            }

            throw new FileNotFoundException("Java runtime not found. Set JAVA_HOME or install Java.");
        }

        public string getVersionManifest()
        {
            return GetVersionManifestAsync().Result;
        }

        public async Task<string> GetVersionManifestAsync()
        {
            string url = "https://bmclapi2.bangbang93.com/mc/game/version_manifest.json";
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public List<LocalVersion> GetLocalVersions()
        {
            List<LocalVersion> versions = new List<LocalVersion>();
            
            string versionsDir = Path.Combine(GameDir, "versions");
            if (!Directory.Exists(versionsDir))
                return versions;

            foreach (string dir in Directory.GetDirectories(versionsDir))
            {
                string versionId = Path.GetFileName(dir);
                
                string jsonPath = Path.Combine(dir, versionId + ".json");
                string jarPath = Path.Combine(dir, versionId + ".jar");
                
                bool hasJson = File.Exists(jsonPath);
                bool hasJar = File.Exists(jarPath);
                
                if (!hasJson && !hasJar)
                    continue;

                string type = "release";
                string releaseTime = "";

                if (hasJson)
                {
                    try
                    {
                        string json = File.ReadAllText(jsonPath);
                        var root = JsonNode.Parse(json);
                        type = root?["type"]?.GetValue<string>() ?? "release";
                        releaseTime = root?["releaseTime"]?.GetValue<string>() ?? "";
                    }
                    catch { }
                }

                versions.Add(new LocalVersion
                {
                    Id = versionId,
                    Type = type,
                    ReleaseTime = releaseTime,
                    HasJson = hasJson,
                    HasJar = hasJar,
                    IsLaunchable = hasJson && hasJar
                });
            }

            return versions.OrderByDescending(v => v.ReleaseTime).ToList();
        }

        public class LocalVersion
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string ReleaseTime { get; set; }
            public bool HasJson { get; set; }
            public bool HasJar { get; set; }
            public bool IsLaunchable { get; set; }
        }

        public void ParseVersionManifest()
        {
            ParseVersionManifestAsync().Wait();
        }

        public async Task ParseVersionManifestAsync()
        {
            string vm = await GetVersionManifestAsync();
            string lastRelease = GetValueFromJson(vm, "latest.release");
            string lastSnapshot = GetValueFromJson(vm, "latest.snapshot");
            Console.WriteLine(lastRelease);
            Console.WriteLine(lastSnapshot);

            var root = JsonNode.Parse(vm);
            var versions = root?["versions"]?.AsArray();
            if (versions == null) return;

            foreach (var version in versions)
            {
                string id = version?["id"]?.GetValue<string>();
                string type = version?["type"]?.GetValue<string>();
                string url = version?["url"]?.GetValue<string>();
                Console.WriteLine(string.Format("Version:{0};{1};{2}", id, type, url));
            }
        }

        public async Task<string> DownloadVersionJson(string versionId)
        {
            string vm = await GetVersionManifestAsync();
            var root = JsonNode.Parse(vm);
            var versions = root?["versions"]?.AsArray();
            if (versions == null) return null;

            foreach (var version in versions)
            {
                string id = version?["id"]?.GetValue<string>();
                if (id != versionId) continue;

                string url = version?["url"]?.GetValue<string>();
                string versionsDir = Path.Combine(GameDir, "versions", versionId);
                Directory.CreateDirectory(versionsDir);

                string jsonPath = Path.Combine(versionsDir, versionId + ".json");
                string jsonContent = await httpClient.GetStringAsync(url);
                File.WriteAllText(jsonPath, jsonContent);
                Console.WriteLine("Downloaded version JSON: " + versionId);
                return jsonContent;
            }

            Console.WriteLine("Version not found: " + versionId);
            return null;
        }

        public async Task DownloadClientJar(string versionJson)
        {
            var root = JsonNode.Parse(versionJson);
            string versionId = root?["id"]?.GetValue<string>();
            var clientNode = root?["downloads"]?["client"];
            if (clientNode == null) return;

            string url = clientNode["url"]?.GetValue<string>();
            string sha1 = clientNode["sha1"]?.GetValue<string>();

            string versionsDir = Path.Combine(GameDir, "versions", versionId);
            Directory.CreateDirectory(versionsDir);

            string jarPath = Path.Combine(versionsDir, versionId + ".jar");
            await DownloadFileWithCheck(url, jarPath, sha1);
            Console.WriteLine("Downloaded client jar: " + Path.GetFileName(jarPath));
        }

        public async Task DownloadLibraries(string versionJson)
        {
            var root = JsonNode.Parse(versionJson);
            var libraries = root?["libraries"]?.AsArray();
            if (libraries == null) return;

            string osName = GetOsName();

            foreach (var lib in libraries)
            {
                if (!IsLibraryAllowed(lib, osName)) continue;

                var artifact = lib?["downloads"]?["artifact"];
                if (artifact == null) continue;

                string url = artifact["url"]?.GetValue<string>();
                string path = artifact["path"]?.GetValue<string>();
                string sha1 = artifact["sha1"]?.GetValue<string>();

                string libPath = Path.Combine(GameDir, "libraries", path);
                await DownloadFileWithCheck(url, libPath, sha1);
                Console.WriteLine("Downloaded library: " + path);
            }
        }

        public async Task DownloadAssets(string versionJson)
        {
            var root = JsonNode.Parse(versionJson);
            var assetIndex = root?["assetIndex"];
            if (assetIndex == null) return;

            string indexUrl = assetIndex["url"]?.GetValue<string>();
            string indexId = assetIndex["id"]?.GetValue<string>();
            string indexSha1 = assetIndex["sha1"]?.GetValue<string>();

            string indexesDir = Path.Combine(GameDir, "assets", "indexes");
            Directory.CreateDirectory(indexesDir);

            string indexPath = Path.Combine(indexesDir, indexId + ".json");
            await DownloadFileWithCheck(indexUrl, indexPath, indexSha1);
            Console.WriteLine("Downloaded asset index: " + indexId);

            string indexContent = File.ReadAllText(indexPath);
            var indexRoot = JsonNode.Parse(indexContent);
            var objects = indexRoot?["objects"]?.AsObject();
            if (objects == null) return;

            string objectsDir = Path.Combine(GameDir, "assets", "objects");
            Directory.CreateDirectory(objectsDir);

            foreach (var obj in objects)
            {
                string hash = obj.Value?["hash"]?.GetValue<string>();
                if (string.IsNullOrEmpty(hash) || hash.Length < 2) continue;
                string hashPrefix = hash.Substring(0, 2);

                string subDir = Path.Combine(objectsDir, hashPrefix);
                Directory.CreateDirectory(subDir);

                string filePath = Path.Combine(subDir, hash);
                string fileUrl = "https://resources.download.minecraft.net/" + hashPrefix + "/" + hash;

                await DownloadFileWithCheck(fileUrl, filePath, hash);
            }
            Console.WriteLine("Downloaded all assets: " + objects.Count);
        }

        private async Task DownloadFileWithCheck(string url, string filePath, string expectedSha1)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(filePath)) return;

            string dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            if (File.Exists(filePath) && !string.IsNullOrEmpty(expectedSha1))
            {
                string actualSha1 = ComputeSha1(filePath);
                if (actualSha1 == expectedSha1) return;
            }

            byte[] data = await httpClient.GetByteArrayAsync(url);
            File.WriteAllBytes(filePath, data);
        }

        private static string ComputeSha1(string filePath)
        {
            using (var sha1 = SHA1.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha1.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }

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

        private static string GetValueFromJson(string json, string path)
        {
            var root = JsonNode.Parse(json);
            if (root == null) return null;

            string[] parts = path.Split('.');
            JsonNode node = root;
            foreach (string part in parts)
            {
                if (node == null) return null;
                node = node[part];
            }
            return node?.GetValue<string>();
        }
    }
}
