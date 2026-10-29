// Made by PapaGordon
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WindowsGSM.Functions;
#if BUILTIN
using Server = WindowsGSM.GameServer.MC;
#else
using Server = WindowsGSM.Plugins.Minecraft_Java;
#endif

class UpdateSafety
{
    static int passed;
    static string fixture;
    static Server plugin;
    static ServerConfig settings;
    static object Call(string name, params object[] args)
    {
        try { return typeof(Server).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(plugin, args); }
        catch (TargetInvocationException e) { throw e.InnerException; }
    }
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    static string Child(string path) { return Path.Combine(fixture, path.Replace('/', Path.DirectorySeparatorChar)); }
    static void Write(string path, string text) { Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path, text); }
    static Dictionary<string, string> Snapshot(string root)
    {
        return Directory.GetFiles(root, "*", SearchOption.AllDirectories).ToDictionary(
            p => p.Substring(root.Length), p => Convert.ToBase64String(File.ReadAllBytes(p)));
    }
    static void Unchanged(string root, Dictionary<string, string> before)
    {
        var after = Snapshot(root);
        Check(before.Count == after.Count && before.All(p => after.ContainsKey(p.Key) && after[p.Key] == p.Value), "Existing server files changed");
    }
    static void Throws<T>(Action action) where T : Exception
    {
        try { action(); } catch (T) { return; }
        throw new Exception("Expected " + typeof(T).Name);
    }
    static void Test(string name, Action action)
    {
        fixture = Path.Combine(Path.GetTempPath(), "minecraft-java-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(fixture);
        settings = new ServerConfig(); plugin = new Server(settings); ServerPath.Root = Child("server/serverfiles");
        try { action(); passed++; Console.WriteLine("PASS " + name); }
        finally
        {
            foreach (string file in Directory.GetFiles(fixture, "*", SearchOption.AllDirectories)) File.SetAttributes(file, FileAttributes.Normal);
            Directory.Delete(fixture, true);
        }
    }
    static string Jar(string path, string id = "1.21.1", int java = 21, bool stable = true)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        using (var zip = ZipFile.Open(path, ZipArchiveMode.Create))
        {
            using (var writer = new StreamWriter(zip.CreateEntry("version.json").Open()))
                writer.Write(new JObject { ["id"] = id, ["java_version"] = java, ["stable"] = stable }.ToString());
            using (var writer = new StreamWriter(zip.CreateEntry("net/minecraft/bundler/Main.class").Open())) writer.Write("fixture");
        }
        return path;
    }
    static JObject Release(string jar)
    {
        var version = (JObject)Call("ReadServerJar", jar);
        string hash;
        using (var sha = SHA1.Create()) using (var file = File.OpenRead(jar))
            hash = BitConverter.ToString(sha.ComputeHash(file)).Replace("-", "").ToLowerInvariant();
        return new JObject
        {
            ["id"] = version["id"].DeepClone(), ["type"] = "release",
            ["javaVersion"] = new JObject { ["majorVersion"] = version["java_version"].DeepClone() },
            ["downloads"] = new JObject { ["server"] = new JObject
            { ["url"] = "https://piston-data.mojang.com/server.jar", ["sha1"] = hash, ["size"] = new FileInfo(jar).Length } }
        };
    }
    static string ExistingServer()
    {
        string root = ServerPath.Root;
        Jar(Path.Combine(root, "server.jar"), "1.20.4", 17);
        Write(Path.Combine(root, ".wgsm-minecraft-version.json"), "old metadata");
        Write(Path.Combine(root, "my world/region/r.0.0.mca"), "WORLD");
        Write(Path.Combine(root, "server.properties"), "motd=My server\nlevel-name=my world\nallow-flight=false\n");
        Write(Path.Combine(root, "user_jvm_args.txt"), "# custom\n-Xms8G\n-Xmx12G\n-XX:+UseG1GC\n");
        Write(Path.Combine(root, "ops.json"), "OPERATOR");
        Write(Path.Combine(root, "whitelist.json"), "WHITELIST");
        Write(Path.Combine(root, "eula.txt"), "# accepted\neula=true\n");
        return root;
    }
    static string Prepare(string source, JObject release, bool accept = false, Func<string, string, Task> download = null)
    {
        if (download == null) download = (url, target) => { File.Copy(source, target); return Task.FromResult(0); };
        return ((Task<string>)Call("PrepareAndCommitUpdate", ServerPath.Root, release, accept, download)).GetAwaiter().GetResult();
    }
    static JObject Manifest()
    {
        return JObject.Parse(@"{'latest':{'release':'26.3','snapshot':'26.4-snapshot-1'},'versions':[
            {'id':'26.4-snapshot-1','type':'snapshot','url':'https://piston-meta.mojang.com/snapshot.json'},
            {'id':'26.3','type':'release','url':'https://piston-meta.mojang.com/26.3.json'},
            {'id':'1.21.1','type':'release','url':'https://piston-meta.mojang.com/1.21.1.json'}]}");
    }
    static void RunChecks(string[] args)
    {
        if (args.Length == 1 && args[0] == "--live-install")
        {
            Test("Install downloads and validates the official JAR automatically", () =>
            {
                plugin.Install().GetAwaiter().GetResult();
                Check(plugin.IsInstallValid(), "Automatic installation failed: " + plugin.Error);
                Check(File.Exists(Path.Combine(ServerPath.Root, "server.jar")), "Download missing");
                Check((bool)Call("IsEulaAccepted", ServerPath.Root), "EULA missing");
            });
            return;
        }
        Test("installation validation retains the original download error", () =>
        {
            plugin.Error = "Minecraft installation/update failed: connection lost";
            Check(!plugin.IsInstallValid() && plugin.Error.EndsWith("connection lost"), "Download error overwritten");
        });
        Test("latest stable selection excludes snapshots and supports exact pins", () =>
        {
            Check((string)((JObject)Call("SelectRelease", Manifest(), "latest-release"))["id"] == "26.3", "Wrong release");
            Check((string)((JObject)Call("SelectRelease", Manifest(), "1.21.1"))["id"] == "1.21.1", "Pin ignored");
            Throws<IOException>(() => Call("SelectRelease", Manifest(), "26.4-snapshot-1"));
            Throws<IOException>(() => Call("SelectRelease", Manifest(), "unknown"));
        });
        Test("unofficial metadata URLs are rejected", () =>
        { var m = Manifest(); m["versions"][1]["url"] = "https://example.com/release.json"; Throws<IOException>(() => Call("SelectRelease", m, "latest-release")); });
        Test("missing hashes and mismatched metadata are rejected", () =>
        {
            var release = Release(Jar(Child("new.jar"))); Throws<IOException>(() => Call("ValidateRelease", release, "wrong"));
            release["downloads"]["server"]["sha1"] = ""; Throws<IOException>(() => Call("ValidateRelease", release, "1.21.1"));
        });
        Test("Java requirement follows the release and rejects unsupported old releases", () =>
        {
            var release = Release(Jar(Child("new.jar"), "26.3", 25)); Call("ValidateRelease", release, "26.3");
            release["javaVersion"]["majorVersion"] = 8; Throws<IOException>(() => Call("ValidateRelease", release, "26.3"));
        });
        Test("Java detection checks major version and 64-bit architecture", () =>
        {
            string java = "java.specification.version = 25\nsun.arch.data.model = 64\n";
            Check((bool)Call("JavaOutputMatches", java, 25), "Java 25 rejected");
            Check(!(bool)Call("JavaOutputMatches", java, 26), "Older Java accepted");
            Check(!(bool)Call("JavaOutputMatches", java.Replace("= 64", "= 32"), 21), "32-bit Java accepted");
            Check(!(bool)Call("JavaOutputMatches", "java.specification.version = 1.8\nsun.arch.data.model = 64", 17), "Java 8 accepted");
        });
        Test("local version is available before a first server start", () =>
        { Jar(Path.Combine(ServerPath.Root, "server.jar")); Check(plugin.GetLocalBuild() == "1.21.1" && plugin.IsInstallValid(), "JAR metadata not read"); });
        Test("import refuses corrupt JARs and snapshots", () =>
        {
            Write(Path.Combine(ServerPath.Root, "server.jar"), "broken"); Check(!plugin.IsImportValid(ServerPath.Root), "Invalid JAR accepted");
            File.Delete(Path.Combine(ServerPath.Root, "server.jar")); Jar(Path.Combine(ServerPath.Root, "server.jar"), "snapshot", 25, false);
            Check(!plugin.IsInstallValid(), "Snapshot accepted");
        });
        Test("declined EULA does not create server files", () =>
        {
            UI.Agree = false;
            try { Check(plugin.Install().GetAwaiter().GetResult() == null && plugin.Error.Contains("EULA") && !Directory.Exists(ServerPath.Root), "Decline ignored"); }
            finally { UI.Agree = true; }
        });
        Test("commented and malformed EULA acceptance block startup", () =>
        {
            Jar(Path.Combine(ServerPath.Root, "server.jar"));
            foreach (string text in new[] { "# eula=true", "eula=true-but-no", "eula=false" })
            { Write(Path.Combine(ServerPath.Root, "eula.txt"), text); Check(plugin.Start().GetAwaiter().GetResult() == null && plugin.Error.Contains("EULA"), "Start ignored EULA"); }
        });
        Test("properties preserve MOTD, world, gameplay, passwords and custom settings", () =>
        {
            ExistingServer(); string file = Path.Combine(ServerPath.Root, "server.properties");
            File.AppendAllText(file, "rcon.password=keep-this\nonline-mode=true\nserver-port=1\nserver-port=2\ncustom=keep\n");
            settings.ServerPort = "25570"; settings.ServerQueryPort = "25571"; settings.ServerMaxPlayer = "42"; plugin.CreateServerCFG();
            string text = File.ReadAllText(file);
            Check(text.Contains("motd=My server") && text.Contains("level-name=my world") && text.Contains("allow-flight=false") && text.Contains("rcon.password=keep-this") && text.Contains("custom=keep"), "Local settings overwritten");
            Check(text.Contains("server-port=25570") && text.Contains("query.port=25571") && text.Contains("max-players=42") && text.Contains("enable-query=true") && text.Split(new[] { "server-port=" }, StringSplitOptions.None).Length == 2, "Managed settings wrong");
        });
        Test("invalid WGSM ports do not rewrite properties", () =>
        { ExistingServer(); var before = Snapshot(ServerPath.Root); settings.ServerPort = "70000"; plugin.CreateServerCFG(); Check(plugin.Error != null, "Bad port accepted"); Unchanged(ServerPath.Root, before); });
        Test("console mode follows AllowsEmbedConsole and preserves existing JVM parameters", () =>
        {
            settings.EmbedConsole = false; settings.ServerParam = "-Xmx8G -Dname=\"My server\""; plugin.AllowsEmbedConsole = true;
            using (var p = (Process)Call("CreateServerProcess", Child("server with spaces"), "C:\\Java JDK\\bin\\java.exe"))
            {
                Check(p.StartInfo.RedirectStandardInput && p.StartInfo.RedirectStandardOutput && p.StartInfo.RedirectStandardError && p.StartInfo.CreateNoWindow, "Streams not redirected");
                Check(p.StartInfo.Arguments == "@user_jvm_args.txt -Xmx8G -Dname=\"My server\" -jar server.jar nogui", "JVM argument order changed");
                Check(!p.StartInfo.UseShellExecute && p.StartInfo.FileName == "C:\\Java JDK\\bin\\java.exe", "Java path quoting wrong");
            }
            plugin.AllowsEmbedConsole = false; settings.EmbedConsole = true;
            using (var p = (Process)Call("CreateServerProcess", fixture, "java.exe")) Check(!p.StartInfo.RedirectStandardInput && !p.StartInfo.RedirectStandardOutput && !p.StartInfo.CreateNoWindow, "Native mode ignored");
        });
        Test("stop sends the save/shutdown command through redirected stdin", () =>
        {
            string script = "$line=[Console]::ReadLine(); if($line -eq 'stop'){exit 0}else{exit 9}";
            using (var p = new Process { StartInfo = new ProcessStartInfo("powershell.exe", "-NoLogo -NoProfile -NonInteractive -EncodedCommand " + Convert.ToBase64String(Encoding.Unicode.GetBytes(script)))
                { UseShellExecute = false, CreateNoWindow = true, RedirectStandardInput = true } })
            {
                p.Start(); plugin.Stop(p).GetAwaiter().GetResult(); Check(p.HasExited && p.ExitCode == 0, "Graceful stop was not sent");
            }
        });
        Test("successful update keeps all user files and a byte-exact runtime backup", () =>
        {
            string root = ExistingServer(), jar = Jar(Child("new.jar")); var before = Snapshot(root); var release = Release(jar); string backup = Prepare(jar, release);
            string jarKey = Path.DirectorySeparatorChar + "server.jar", markerKey = Path.DirectorySeparatorChar + ".wgsm-minecraft-version.json";
            Check(backup != null && Convert.ToBase64String(File.ReadAllBytes(Path.Combine(backup, "server.jar"))) == before[jarKey], "Old runtime backup differs");
            Check(plugin.GetLocalBuild() == "1.21.1" && (bool)Call("InstalledMatches", root, release), "New version not active");
            foreach (var item in before.Where(p => p.Key != jarKey && p.Key != markerKey)) Check(Convert.ToBase64String(File.ReadAllBytes(root + item.Key)) == item.Value, "User data changed: " + item.Key);
            Check(!File.Exists((string)Call("RecoveryMarker", root)), "Recovery marker left on success");
        });
        Test("fresh install creates RAM defaults and accepted EULA", () =>
        {
            string jar = Jar(Child("new.jar")); Prepare(jar, Release(jar), true);
            Check(plugin.IsInstallValid() && (bool)Call("IsEulaAccepted", ServerPath.Root), "Fresh install incomplete");
            Check(File.ReadAllText(Path.Combine(ServerPath.Root, "user_jvm_args.txt")).Contains("-Xmx4G"), "Default RAM missing");
        });
        Test("update never accepts a missing EULA", () =>
        { string jar = Jar(Child("new.jar")); Prepare(jar, Release(jar)); Check(!(bool)Call("IsEulaAccepted", ServerPath.Root), "Update silently accepted EULA"); });
        Test("same version with different bytes is not considered current", () =>
        {
            string jar = Jar(Child("new.jar")); var release = Release(jar); Prepare(jar, release);
            using (var stream = File.Open(Path.Combine(ServerPath.Root, "server.jar"), FileMode.Append)) stream.WriteByte(1);
            Check(!(bool)Call("InstalledMatches", ServerPath.Root, release), "Changed JAR skipped");
        });
        Test("download failure leaves every existing server file untouched", () =>
        {
            ExistingServer(); var before = Snapshot(ServerPath.Root); string jar = Jar(Child("new.jar"));
            Throws<IOException>(() => Prepare(jar, Release(jar), false, (url, target) => { Write(target, "partial"); throw new IOException("connection lost"); })); Unchanged(ServerPath.Root, before);
        });
        Test("wrong hash or length leaves the old server untouched", () =>
        {
            ExistingServer(); var before = Snapshot(ServerPath.Root); string jar = Jar(Child("new.jar")); var release = Release(jar);
            release["downloads"]["server"]["sha1"] = new string('0', 40); Throws<IOException>(() => Prepare(jar, release)); Unchanged(ServerPath.Root, before);
            release = Release(jar); release["downloads"]["server"]["size"] = 1; Throws<IOException>(() => Prepare(jar, release)); Unchanged(ServerPath.Root, before);
        });
        Test("JAR version mismatch is rejected before activation", () =>
        { ExistingServer(); var before = Snapshot(ServerPath.Root); string jar = Jar(Child("new.jar")); var release = Release(jar); release["id"] = "26.3"; Throws<IOException>(() => Prepare(jar, release)); Unchanged(ServerPath.Root, before); });
        Test("read-only runtime is rejected without changing live files", () =>
        {
            ExistingServer(); var before = Snapshot(ServerPath.Root); string jar = Jar(Child("new.jar")); File.SetAttributes(Path.Combine(ServerPath.Root, "server.jar"), FileAttributes.ReadOnly);
            Throws<IOException>(() => Prepare(jar, Release(jar))); Unchanged(ServerPath.Root, before);
        });
        Test("activation failure restores already replaced runtime files", () =>
        {
            ExistingServer(); var before = Snapshot(ServerPath.Root); string jar = Jar(Child("new.jar"));
            using (var locked = new FileStream(Path.Combine(ServerPath.Root, ".wgsm-minecraft-version.json"), FileMode.Open, FileAccess.Read, FileShare.Read)) Throws<IOException>(() => Prepare(jar, Release(jar)));
            Unchanged(ServerPath.Root, before); Check(!File.Exists((string)Call("RecoveryMarker", ServerPath.Root)), "Successful rollback left recovery marker");
        });
        Test("recovery marker blocks server start and another update", () =>
        {
            ExistingServer(); Write((string)Call("RecoveryMarker", ServerPath.Root), "manual recovery");
            Throws<IOException>(() => ((FileStream)Call("AcquireUpdateLock", ServerPath.Root)).Dispose());
            Check(plugin.Start().GetAwaiter().GetResult() == null && plugin.Error.Contains("recovery"), "Start ignored recovery");
        });
        Test("update lock excludes concurrent operations", () =>
        { using ((FileStream)Call("AcquireUpdateLock", ServerPath.Root)) Throws<IOException>(() => ((FileStream)Call("AcquireUpdateLock", ServerPath.Root)).Dispose()); });
        if (args.Length == 3)
        {
            Test("real Mojang manifest selects the live stable release", () =>
            {
                var manifest = JObject.Parse(File.ReadAllText(args[1])); var release = JObject.Parse(File.ReadAllText(args[2])); var selected = (JObject)Call("SelectRelease", manifest, "latest-release");
                Check((string)selected["id"] == (string)release["id"], "Live manifest differs"); Call("ValidateRelease", release, (string)selected["id"]);
            });
            Test("real server passes size, SHA-1, import and local version checks", () =>
            {
                var release = JObject.Parse(File.ReadAllText(args[2])); Prepare(args[0], release, true);
                Check(plugin.IsImportValid(ServerPath.Root) && plugin.GetLocalBuild() == (string)release["id"] && (bool)Call("InstalledMatches", ServerPath.Root, release), "Official server validation failed");
            });
        }
    }
    static int Main(string[] args)
    {
        try { RunChecks(args); Console.WriteLine(passed + " Minecraft checks passed. No game server or firewall rule was started or changed."); return 0; }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
