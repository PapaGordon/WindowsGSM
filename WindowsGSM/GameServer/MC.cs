// Made by PapaGordon
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Query;

namespace WindowsGSM.GameServer
{
    class MC
    {
        private readonly ServerConfig _serverData;
        public string Error, Notice;
        public MC(ServerConfig serverData) { _serverData = serverData; }

        public string StartPath => "server.jar";
        public const string FullName = "Minecraft: Java Edition Server";
        public bool AllowsEmbedConsole = true;
        public int PortIncrements = 1;
        public object QueryMethod = new UT3();
        public string Port = "25565";
        public string QueryPort = "25565";
        public string Defaultmap = "world";
        public string Maxplayers = "20";
        public string Additional = "";

        private const string ManifestUrl = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
        private const string VersionMarker = ".wgsm-minecraft-version.json";
        private const string DefaultJvmArgs = "# Made by PapaGordon\r\n# Adjust RAM here. WindowsGSM parameters can override these Java options.\r\n-Xms2G\r\n-Xmx4G\r\n";

        public void CreateServerCFG()
        {
            Error = null;
            try { EnsureServerProperties(ServerPath.GetServersServerFiles(_serverData.ServerID)); }
            catch (Exception e) { Error = "Unable to update server.properties: " + e.Message; }
        }

        public async Task<Process> Start()
        {
            Error = null;
            Notice = null;
            try
            {
                string root = ServerPath.GetServersServerFiles(_serverData.ServerID);
                using (AcquireUpdateLock(root))
                {
                    JObject installed = ReadServerJar(Path.Combine(root, StartPath));
                    if (!IsEulaAccepted(root))
                        throw new IOException("Minecraft EULA is not accepted. Set eula=true in eula.txt first.");
                    string java = await Task.Run(() => FindJava((int)installed["java_version"]));
                    if (java == null) return null;
                    EnsureServerProperties(root);
                    string jvmArgs = Path.Combine(root, "user_jvm_args.txt");
                    if (!File.Exists(jvmArgs)) File.WriteAllText(jvmArgs, DefaultJvmArgs);
                    Process p = CreateServerProcess(root, java);
                    try
                    {
                        p.Start();
                        if (AllowsEmbedConsole)
                        {
                            p.BeginOutputReadLine();
                            p.BeginErrorReadLine();
                        }
                        return p;
                    }
                    catch { p.Dispose(); throw; }
                }
            }
            catch (Exception e) { Error = "Minecraft could not start: " + e.Message; return null; }
        }

        private Process CreateServerProcess(string root, string java)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = root,
                    FileName = java,
                    Arguments = "@user_jvm_args.txt " +
                        (string.IsNullOrWhiteSpace(_serverData.ServerParam) ? "" : _serverData.ServerParam + " ") +
                        "-jar server.jar nogui",
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false,
                    CreateNoWindow = AllowsEmbedConsole,
                    RedirectStandardInput = AllowsEmbedConsole,
                    RedirectStandardOutput = AllowsEmbedConsole,
                    RedirectStandardError = AllowsEmbedConsole
                },
                EnableRaisingEvents = true
            };
            // WindowsGSM sets AllowsEmbedConsole immediately before Start().
            if (AllowsEmbedConsole)
            {
                var console = new ServerConsole(_serverData.ServerID);
                p.OutputDataReceived += console.AddOutput;
                p.ErrorDataReceived += console.AddOutput;
            }
            return p;
        }

        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                if (p == null || p.HasExited) return;
                try
                {
                    if (p.StartInfo.RedirectStandardInput)
                    {
                        p.StandardInput.WriteLine("stop");
                        p.StandardInput.Flush();
                    }
                    else ServerConsole.SendMessageToMainWindow(p.MainWindowHandle, "stop");
                    if (!p.WaitForExit(30000)) p.Kill();
                }
                catch
                {
                    try { if (!p.HasExited) p.Kill(); }
                    catch { }
                }
            });
        }

        public async Task<Process> Install()
        {
            Error = null;
            Notice = null;
            if (!await UI.CreateYesNoPromptV1("Minecraft EULA",
                "By continuing you confirm that you agree to the Minecraft EULA.\nhttps://www.minecraft.net/eula", "Agree", "Decline"))
            {
                Error = "Minecraft EULA was not accepted.";
                return null;
            }
            return await InstallOrUpdate(true, false);
        }

        public Task<Process> Update(bool validate = false, string custom = null)
        {
            // The fork passes these arguments; version selection uses minecraft-version.txt.
            return InstallOrUpdate(false, validate);
        }

        private async Task<Process> InstallOrUpdate(bool acceptEula, bool validate)
        {
            Error = null;
            Notice = null;
            try
            {
                string root = ServerPath.GetServersServerFiles(_serverData.ServerID);
                using (AcquireUpdateLock(root))
                {
                    JObject release = await GetSelectedRelease(root);
                    if (!acceptEula && !validate && InstalledMatches(root, release))
                    {
                        Notice = "Minecraft " + (string)release["id"] + " is already up to date.";
                        return null;
                    }
                    string backup = await PrepareAndCommitUpdate(root, release, acceptEula, DownloadServer);
                    Notice = "Minecraft " + (string)release["id"] + " installed." +
                        (backup == null ? "" : " Previous runtime files: " + backup);
                }
            }
            catch (Exception e) { Error = "Minecraft installation/update failed: " + e.Message; }
            // Downloads finish here. WindowsGSM must not try to read an installer process.
            return null;
        }

        public bool IsInstallValid()
        {
            // Keep the download error; WindowsGSM shows it after this check.
            if (!string.IsNullOrWhiteSpace(Error)) return false;
            return IsImportValid(ServerPath.GetServersServerFiles(_serverData.ServerID));
        }

        public bool IsImportValid(string path)
        {
            Error = null;
            try { ReadServerJar(Path.Combine(path, StartPath)); return true; }
            catch (Exception e) { Error = "A Vanilla server.jar for Minecraft 1.18 or newer is required: " + e.Message; return false; }
        }

        public string GetLocalBuild()
        {
            try { return (string)ReadServerJar(ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath))["id"]; }
            catch { return string.Empty; }
        }

        public async Task<string> GetRemoteBuild()
        {
            try
            {
                Error = null;
                return (string)(await GetSelectedRelease(ServerPath.GetServersServerFiles(_serverData.ServerID)))["id"];
            }
            catch (Exception e) { Error = "Unable to get the Minecraft release: " + e.Message; return string.Empty; }
        }

        private async Task<JObject> GetSelectedRelease(string root)
        {
            string selectionFile = Path.Combine(root, "minecraft-version.txt");
            string selection = File.Exists(selectionFile) ? File.ReadAllText(selectionFile).Trim() : "latest-release";
            JObject manifest = JObject.Parse(await DownloadJson(ManifestUrl));
            JObject entry = SelectRelease(manifest, selection);
            JObject release = JObject.Parse(await DownloadJson((string)entry["url"]));
            ValidateRelease(release, (string)entry["id"]);
            return release;
        }

        private JObject SelectRelease(JObject manifest, string selection)
        {
            string id = string.Equals(selection, "latest-release", StringComparison.OrdinalIgnoreCase)
                ? (string)manifest["latest"]["release"] : selection;
            var versions = manifest["versions"] as JArray;
            JObject entry = versions == null ? null : versions.OfType<JObject>().FirstOrDefault(v =>
                (string)v["id"] == id && (string)v["type"] == "release");
            if (entry == null) throw new IOException("Release '" + selection + "' was not found. Use latest-release or an exact stable version in minecraft-version.txt.");
            RequireMojangUrl((string)entry["url"]);
            return entry;
        }

        private void ValidateRelease(JObject release, string expected)
        {
            var server = release.SelectToken("downloads.server") as JObject;
            if ((string)release["id"] != expected || (string)release["type"] != "release" || server == null ||
                !Regex.IsMatch((string)server["sha1"] ?? "", "\\A[0-9a-fA-F]{40}\\z") || (long?)server["size"] <= 0 || server["size"] == null)
                throw new IOException("Mojang returned incomplete or mismatched server download data.");
            if ((int?)release.SelectToken("javaVersion.majorVersion") < 17 || release.SelectToken("javaVersion.majorVersion") == null)
                throw new IOException("This plugin supports Vanilla releases from Minecraft 1.18 (Java 17) onwards.");
            RequireMojangUrl((string)server["url"]);
        }

        private static void RequireMojangUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri) || uri.Scheme != "https" ||
                !uri.Host.EndsWith(".mojang.com", StringComparison.OrdinalIgnoreCase))
                throw new IOException("Expected an official Mojang HTTPS download URL.");
        }

        private async Task<string> DownloadJson(string url)
        {
            RequireMojangUrl(url);
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
                return await client.GetStringAsync(url);
        }

        private async Task DownloadServer(string url, string destination)
        {
            RequireMojangUrl(url);
            using (var client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) })
            using (var response = await client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                using (var stream = File.Create(destination)) await response.Content.CopyToAsync(stream);
            }
        }

        private JObject ReadServerJar(string path)
        {
            using (var jar = ZipFile.OpenRead(path))
            {
                var entry = jar.GetEntry("version.json");
                if (entry == null || entry.Length > 65536 || jar.GetEntry("net/minecraft/bundler/Main.class") == null)
                    throw new IOException("The file is not a supported Vanilla server JAR.");
                using (var reader = new StreamReader(entry.Open()))
                {
                    JObject version = JObject.Parse(reader.ReadToEnd());
                    if (string.IsNullOrWhiteSpace((string)version["id"]) || (int?)version["java_version"] < 17 ||
                        version["java_version"] == null || (bool?)version["stable"] != true)
                        throw new IOException("The JAR must be a stable Vanilla release using Java 17 or newer.");
                    return version;
                }
            }
        }

        private bool InstalledMatches(string root, JObject release)
        {
            try
            {
                return GetLocalBuild() == (string)release["id"] &&
                    FileMatches(Path.Combine(root, StartPath), (JObject)release["downloads"]["server"]);
            }
            catch { return false; }
        }

        private bool FileMatches(string path, JObject download)
        {
            if (!File.Exists(path) || new FileInfo(path).Length != (long)download["size"]) return false;
            using (var sha = SHA1.Create())
            using (var stream = File.OpenRead(path))
                return string.Equals(BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", ""),
                    (string)download["sha1"], StringComparison.OrdinalIgnoreCase);
        }

        private async Task<string> PrepareAndCommitUpdate(string root, JObject release, bool acceptEula,
            Func<string, string, Task> download)
        {
            ValidateRelease(release, (string)release["id"]);
            string parent = Path.GetDirectoryName(Path.GetFullPath(root));
            string stage = Path.Combine(parent, ".wgsm-minecraft-download-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(stage);
            try
            {
                string jarPath = Path.Combine(stage, StartPath);
                var server = (JObject)release["downloads"]["server"];
                await download((string)server["url"], jarPath);
                if (!FileMatches(jarPath, server)) throw new IOException("Server download failed its size/SHA-1 check. The existing server was kept.");
                JObject jarVersion = ReadServerJar(jarPath);
                if ((string)jarVersion["id"] != (string)release["id"] || (int)jarVersion["java_version"] != (int)release["javaVersion"]["majorVersion"])
                    throw new IOException("The server JAR does not match Mojang's release metadata.");
                File.WriteAllText(Path.Combine(stage, VersionMarker), new JObject
                {
                    ["id"] = release["id"].DeepClone(),
                    ["javaVersion"] = release["javaVersion"].DeepClone(),
                    ["downloads"] = new JObject { ["server"] = server.DeepClone() }
                }.ToString());
                if (!File.Exists(Path.Combine(root, "user_jvm_args.txt")))
                    File.WriteAllText(Path.Combine(stage, "user_jvm_args.txt"), DefaultJvmArgs);
                if (acceptEula && !IsEulaAccepted(root))
                    File.WriteAllText(Path.Combine(stage, "eula.txt"), "eula=true" + Environment.NewLine);
                return PublishPreparedUpdate(root, stage);
            }
            finally
            {
                // Only this call's download directory is removed; worlds are never copied or deleted.
                if (Directory.Exists(stage)) Directory.Delete(stage, true);
            }
        }

        private string PublishPreparedUpdate(string root, string stage)
        {
            Directory.CreateDirectory(root);
            RejectLink(root);
            string[] files = Directory.GetFiles(stage).Select(Path.GetFileName).ToArray();
            var existing = new HashSet<string>(files.Where(name => File.Exists(Path.Combine(root, name))));
            foreach (string name in files)
            {
                string target = Path.Combine(root, name);
                if (Directory.Exists(target)) throw new IOException("A directory blocks the runtime file: " + target);
                if (File.Exists(target))
                {
                    RejectLink(target);
                    if ((File.GetAttributes(target) & FileAttributes.ReadOnly) != 0)
                        throw new IOException("A runtime file is read-only: " + target);
                }
            }
            string backupRoot = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(root)), "minecraft-update-backups");
            Directory.CreateDirectory(backupRoot);
            RejectLink(backupRoot);
            string backup = Path.Combine(backupRoot, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(backup);
            foreach (string name in existing) File.Copy(Path.Combine(root, name), Path.Combine(backup, name));
            string marker = RecoveryMarker(root);
            using (var stream = new FileStream(marker, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                byte[] recovery = Encoding.UTF8.GetBytes("Minecraft runtime recovery\r\nServer: " + root +
                    "\r\nPrevious runtime files: " + backup + "\r\nFiles being installed: " + string.Join(", ", files) +
                    "\r\nStop the server. Restore the backed-up files to Server and remove newly added runtime files without a backup.\r\n" +
                    "Worlds and settings were not included in this runtime update.\r\nRemove this recovery file after checking the server.\r\n");
                stream.Write(recovery, 0, recovery.Length);
                stream.Flush(true);
            }
            var published = new List<string>();
            try
            {
                foreach (string name in files)
                {
                    string target = Path.Combine(root, name);
                    if (existing.Contains(name)) File.Replace(Path.Combine(stage, name), target, null);
                    else File.Move(Path.Combine(stage, name), target);
                    published.Add(name);
                }
            }
            catch (Exception publishError)
            {
                try
                {
                    foreach (string name in published)
                    {
                        string target = Path.Combine(root, name);
                        if (existing.Contains(name)) File.Copy(Path.Combine(backup, name), target, true);
                        else File.Delete(target);
                    }
                    File.Delete(marker);
                }
                catch (Exception restoreError)
                {
                    throw new IOException("The update needs manual recovery. Read " + marker + ". " + restoreError.Message, publishError);
                }
                throw new IOException("Could not activate the update. Previous runtime files were restored. " + publishError.Message, publishError);
            }
            File.Delete(marker);
            return existing.Count == 0 ? null : backup;
        }

        private void RejectLink(string path)
        {
            for (string current = Path.GetFullPath(path); current != null; current = Path.GetDirectoryName(current))
                if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                    throw new IOException("Linked runtime files/directories need a manual installation: " + current);
        }

        private string RecoveryMarker(string root)
        {
            return Path.Combine(Path.GetDirectoryName(Path.GetFullPath(root)), ".wgsm-minecraft-update-recovery.txt");
        }

        private FileStream AcquireUpdateLock(string root)
        {
            string parent = Path.GetDirectoryName(Path.GetFullPath(root));
            Directory.CreateDirectory(parent);
            RejectLink(parent);
            if (Directory.Exists(root)) RejectLink(root);
            var updateLock = new FileStream(Path.Combine(parent, ".wgsm-minecraft-update.lock"),
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            if (File.Exists(RecoveryMarker(root)))
            {
                updateLock.Dispose();
                throw new IOException("An interrupted Minecraft update needs recovery. Read " + RecoveryMarker(root));
            }
            return updateLock;
        }

        private bool IsEulaAccepted(string root)
        {
            string file = Path.Combine(root, "eula.txt");
            if (!File.Exists(file)) return false;
            string assignment = File.ReadAllLines(file).LastOrDefault(line => Regex.IsMatch(line, @"^\s*eula\s*[=:]"));
            return assignment != null && Regex.IsMatch(assignment, @"^\s*eula\s*[=:]\s*true\s*$", RegexOptions.IgnoreCase);
        }

        private string FindJava(int required)
        {
            var candidates = new List<string>();
            string javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrWhiteSpace(javaHome)) candidates.Add(Path.Combine(javaHome.Trim().Trim('"'), "bin", "java.exe"));
            string programFiles = Environment.GetEnvironmentVariable("ProgramW6432") ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            foreach (string vendor in new[] { "Java", "Eclipse Adoptium", "Microsoft", "Amazon Corretto", "BellSoft", "Zulu" })
            {
                string folder = Path.Combine(programFiles, vendor);
                try
                {
                    if (Directory.Exists(folder))
                        candidates.AddRange(Directory.GetDirectories(folder).OrderByDescending(d => d)
                            .Select(d => Path.Combine(d, "bin", "java.exe")));
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
            foreach (string folder in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
            {
                try { if (!string.IsNullOrWhiteSpace(folder)) candidates.Add(Path.Combine(folder.Trim().Trim('"'), "java.exe")); }
                catch (ArgumentException) { }
            }
            foreach (string candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
                if (File.Exists(candidate) && IsSuitableJava(candidate, required)) return candidate;
            Error = "64-bit Java " + required + " or newer was not found. Install a matching JDK and set JAVA_HOME or PATH. " +
                "The Java requirement follows the selected Minecraft release.";
            return null;
        }

        private bool IsSuitableJava(string path, int required)
        {
            try
            {
                using (var p = new Process
                {
                    StartInfo = new ProcessStartInfo(path, "-XshowSettings:properties -version")
                    {
                        UseShellExecute = false, CreateNoWindow = true,
                        RedirectStandardOutput = true, RedirectStandardError = true
                    }
                })
                {
                    p.Start();
                    Task<string> output = p.StandardOutput.ReadToEndAsync(), error = p.StandardError.ReadToEndAsync();
                    if (!p.WaitForExit(10000)) { p.Kill(); return false; }
                    return p.ExitCode == 0 && JavaOutputMatches(output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult(), required);
                }
            }
            catch { return false; }
        }

        private bool JavaOutputMatches(string text, int required)
        {
            Match version = Regex.Match(text, @"java\.specification\.version\s*=\s*(\d+)(?:\.(\d+))?");
            if (!version.Success) return false;
            int major = int.Parse(version.Groups[1].Value);
            if (major == 1 && version.Groups[2].Success) major = int.Parse(version.Groups[2].Value);
            return major >= required && Regex.IsMatch(text, @"sun\.arch\.data\.model\s*=\s*64\b");
        }

        private void EnsureServerProperties(string root)
        {
            Directory.CreateDirectory(root);
            string path = Path.Combine(root, "server.properties");
            var lines = File.Exists(path) ? File.ReadAllLines(path).ToList() : new List<string>();
            if (!lines.Any(l => Regex.IsMatch(l, @"^\s*motd\s*[=:]"))) SetProperty(lines, "motd", "Minecraft: Java Edition (Vanilla)");
            SetProperty(lines, "server-port", ValidNumber(_serverData.ServerPort, 25565, 65535).ToString());
            SetProperty(lines, "enable-query", "true");
            SetProperty(lines, "query.port", ValidNumber(_serverData.ServerQueryPort, 25565, 65535).ToString());
            SetProperty(lines, "max-players", ValidNumber(_serverData.ServerMaxPlayer, 20, int.MaxValue).ToString());
            // Existing world names, MOTD, gameplay, RCON and bind settings stay as they are.
            File.WriteAllLines(path, lines.ToArray(), new UTF8Encoding(false));
        }

        private int ValidNumber(string value, int fallback, int maximum)
        {
            int number;
            if (string.IsNullOrWhiteSpace(value)) return fallback;
            if (int.TryParse(value, out number) && number > 0 && number <= maximum) return number;
            throw new IOException("Invalid WindowsGSM port/player setting: " + value);
        }

        private void SetProperty(List<string> lines, string key, string value)
        {
            string pattern = @"^\s*" + Regex.Escape(key) + @"\s*[=:]";
            int first = lines.FindIndex(line => Regex.IsMatch(line, pattern));
            lines.RemoveAll(line => Regex.IsMatch(line, pattern));
            lines.Insert(first < 0 ? lines.Count : Math.Min(first, lines.Count), key + "=" + value);
        }
    }
}
