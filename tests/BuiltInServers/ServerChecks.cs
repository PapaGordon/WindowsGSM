// Made by PapaGordon
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using WindowsGSM.Functions;

public class CustomPlugin
{
    public CustomPlugin(ServerConfig data) { }
}

internal static class ServerChecks
{
    private static int passed;
    private static readonly Assembly Fork = typeof(ServerConfig).Assembly;
    private static readonly Type Factory = Fork.GetType("WindowsGSM.GameServer.Data.Class", true);
    private static readonly Type Catalog = Fork.GetType("WindowsGSM.GameServer.Data.BuiltInServers", true);
    private static readonly string[] Names = {
        "Arma 3 Dedicated Server", "Minecraft: All the Mods 10 (ATM10)", "Enshrouded Dedicated Server",
        "Farming Simulator 22 Dedicated Server", "Farming Simulator 25 Dedicated Server", "Project Zomboid Dedicated Server",
        "SCP: Secret Laboratory Dedicated Server", "Valheim Dedicated Server"
    };
    private static readonly string[] Files = { "ARMA3", "ATM10", "Enshrouded", "FarmingSimulator22", "FarmingSimulator25", "ProjectZomboid", "SCPSL", "Valheim" };

    private static void Check(bool condition, string description)
    {
        if (!condition) throw new Exception(description);
        passed++;
        Console.WriteLine("PASS " + description);
    }

    private static object Create(string name, ServerConfig data = null, List<PluginMetadata> plugins = null)
    {
        return Factory.GetMethod("Get", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { name, data, plugins });
    }

    private static ServerConfig Settings(string id)
    {
        return new ServerConfig(id) {
            ServerName = "MeFriendos \"DayZ\" | Grüße", ServerIP = "0.0.0.0", ServerPort = "2402", ServerQueryPort = "2405",
            ServerMaxPlayer = "42", ServerMap = "dayzOffline.chernarusplus",
            ServerParam = "-config=serverDZ.cfg -profiles=profile -doLogs -adminLog -freezeCheck"
        };
    }

    private static ProcessStartInfo Prepare(object dayz)
    {
        try { return (ProcessStartInfo)dayz.GetType().GetMethod("PrepareStart", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(dayz, null); }
        catch (TargetInvocationException e) { throw e.InnerException; }
    }

    private static void Reject(object dayz, string description)
    {
        try { Prepare(dayz); }
        catch (Exception e) when (e is InvalidOperationException || e is IOException || e is UnauthorizedAccessException || e is ArgumentException)
        { Check(true, description); return; }
        throw new Exception("Expected rejection: " + description);
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CommandLineToArgvW(string command, out int count);
    [DllImport("kernel32.dll")] private static extern IntPtr LocalFree(IntPtr memory);

    private static string[] Arguments(ProcessStartInfo info)
    {
        int count;
        IntPtr pointer = CommandLineToArgvW("DayZServer_x64.exe " + info.Arguments, out count);
        if (pointer == IntPtr.Zero) throw new Exception("Windows argument parser failed.");
        try { return Enumerable.Range(1, count - 1).Select(i => Marshal.PtrToStringUni(Marshal.ReadIntPtr(pointer, i * IntPtr.Size))).ToArray(); }
        finally { LocalFree(pointer); }
    }

    private static Dictionary<string, string> Icons()
    {
        using (var stream = Fork.GetManifestResourceStream("WindowsGSM.GameServer.Data.Icon.resources"))
        using (var reader = new ResourceReader(stream))
            return reader.Cast<DictionaryEntry>().ToDictionary(e => (string)e.Key, e => (string)e.Value);
    }

    private static void CheckCatalog()
    {
        var icons = Icons();
        using (var stream = Fork.GetManifestResourceStream("WindowsGSM.g.resources"))
        using (var reader = new ResourceReader(stream))
        {
            var embedded = reader.Cast<DictionaryEntry>().Select(e => (string)e.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < Names.Length; i++)
            {
                dynamic server = Create(Names[i]);
                Check(server != null && server.GetType().Assembly == Fork && server.FullName == Names[i], Files[i] + " is built in without plugin files");
                Check(Create(Names[i] + " [" + Files[i] + ".cs]").GetType() == server.GetType(), Files[i] + " keeps the old servergame identifier");
                Check(icons.ContainsKey(Names[i]) && embedded.Contains(icons[Names[i]]), Files[i] + " has an embedded selection icon");
                Check(Fork.GetManifestResourceNames().Any(n => n.EndsWith("WindowsGSM." + Files[i] + ".txt", StringComparison.Ordinal)), Files[i] + " license is embedded in the assembly");
                Check((bool)Catalog.GetMethod("Contains", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { Names[i] + " [" + Files[i] + ".cs]" }), Files[i] + " duplicate selection is recognized");
                Check(server.GetType().GetMethod("Start") != null && server.GetType().GetMethod("Install") != null
                    && server.GetType().GetMethod("Stop") != null && server.GetType().GetMethod("IsImportValid") != null, Files[i] + " exposes the install/import/start/stop API");
            }
        }
        Check(Create("DayZ Dedicated Server").GetType().FullName == "WindowsGSM.GameServer.DAYZ", "DayZ keeps its original built-in entry");
        Check(Create("DayZ Dedicated Server [DayZ.cs]").GetType().FullName == "WindowsGSM.GameServer.DAYZ", "Standalone DayZ identifiers resolve to the existing built-in entry");
        Check(icons.Keys.Count(k => k.StartsWith("DayZ", StringComparison.Ordinal)) == 1, "DayZ has one selection entry");
        Check(Create("Missing game") == null && Create(null) == null, "Unknown games are still rejected");
        Check(Create("Other server", plugins: new List<PluginMetadata> { new PluginMetadata { IsLoaded = true, FullName = "Other server", Type = typeof(CustomPlugin) } }) is CustomPlugin,
            "Third-party plugin fallback still works");
    }

    private static string[] FirewallPaths(string id, bool enabled)
    {
        return ((IEnumerable<string>)Fork.GetType("WindowsGSM.Functions.ServerFirewall", true)
            .GetMethod("GetProgramPaths", BindingFlags.NonPublic | BindingFlags.Static)
            .Invoke(null, new object[] { id, "DayZ Dedicated Server", "DayZServer_x64.exe", enabled })).ToArray();
    }

    private static async Task CheckDayZ(string id, string secondId, Func<ServerConfig, object> create)
    {
        var settings = Settings(id);
        dynamic dayz = create(settings);
        string root = ServerPath.GetServersServerFiles(id);
        Directory.CreateDirectory(Path.Combine(root, "mpmissions", settings.ServerMap));
        Directory.CreateDirectory(ServerPath.GetServersConfigs(id));
        string path = Path.Combine(root, "serverDZ.cfg");
        ProcessStartInfo start = Prepare(dayz);
        string text = File.ReadAllText(path);
        Check(text.Contains("hostname = \"MeFriendos \"\"DayZ\"\" | Grüße\";"), "DayZ writes and escapes the WGSM name");
        Check(text.Contains("maxPlayers = 42;") && text.Contains("steamQueryPort = 2405;"), "DayZ writes actual slots and query port");
        Check(text.Contains("verifySignatures = 2;") && text.Contains("passwordAdmin = \"\";"), "DayZ defaults use signature checking and no shared admin password");
        var args = Arguments(start);
        Check(args.Contains("-port=2402") && args.Contains("-ip=0.0.0.0"), "Game port and bind address reach Windows arguments");
        Check(args.Contains("-config=" + path) && args.Contains("-profiles=" + Path.Combine(root, "profile")), "Config and profile paths survive Windows quoting");
        Check(args.Contains("-mission=" + Path.Combine(root, "mpmissions", settings.ServerMap)), "WGSM map selects the installed mission");
        Check(!start.RedirectStandardInput && !start.RedirectStandardOutput && !start.UseShellExecute && !start.CreateNoWindow && start.WindowStyle == ProcessWindowStyle.Minimized,
            "DayZ keeps a discoverable minimized native window without stdin/output redirection");
        DateTime unchanged = File.GetLastWriteTimeUtc(path);
        Prepare(dayz);
        Check(File.GetLastWriteTimeUtc(path) == unchanged && !File.Exists(path + ".bak"), "Unchanged DayZ settings do not rewrite the file");

        string original = "// hostname = \"comment\";\r\n/* steamQueryPort=1; { } */\r\nhostname=\"old\"; // keep this\r\nmaxPlayers=10;\r\nsteamQueryPort=2305;\r\n"
            + "password=\"keep;{}\"; passwordAdmin=\"private\"; motd[]={\"hostname=bad;\",\"Hello\"};\r\n"
            + "class Missions { class DayZ { template=\"custom\"; hostname=\"nested\"; }; };\r\nserverTimeAcceleration=6;\r\n";
        File.WriteAllText(path, original);
        Prepare(dayz);
        text = File.ReadAllText(path);
        Check(text.Contains("// hostname = \"comment\";") && text.Contains("/* steamQueryPort=1; { } */") && text.Contains("// keep this"), "DayZ preserves comments");
        Check(text.Contains("password=\"keep;{}\";") && text.Contains("passwordAdmin=\"private\";") && text.Contains("serverTimeAcceleration=6;"), "DayZ preserves passwords and gameplay settings");
        Check(text.Contains("template=\"custom\"; hostname=\"nested\";") && text.Contains("motd[]={\"hostname=bad;\",\"Hello\"}"), "DayZ does not rewrite nested classes or quoted MOTD text");
        Check(File.ReadAllText(path + ".bak") == original && Directory.GetFiles(root, "*.tmp").Length == 0, "Changed configuration has an exact backup and no leftover temporary file");

        File.WriteAllText(path, "hostname=\"one\"; hostname=\"two\"; maxPlayers=1; steamQueryPort=1;");
        Prepare(dayz);
        Check(Regex.Matches(File.ReadAllText(path), "hostname= ").Count == 2 && !File.ReadAllText(path).Contains("\"two\""), "Duplicate managed assignments get consistent values");
        File.WriteAllText(path, "// custom settings only\r\npassword=\"private\";");
        Prepare(dayz);
        Check(File.ReadAllText(path).Contains("steamQueryPort = 2405;") && File.ReadAllText(path).Contains("password=\"private\";"), "Missing managed keys are appended without replacing the config");

        settings.ServerParam = "-config=serverDZ.cfg -profiles=\"profile with spaces\\\" -port=9999 -ip=1.2.3.4 -mission=wrong -mod=\"@Manual;@Shared\" -serverMod=\"@Server Only\" -freezeCheck";
        File.WriteAllText(ServerPath.GetServersConfigs(id, "DayZActivatedMods.cfg"), "@Shared\r\n@Selected\r\n\r\n@selected\r\n");
        args = Arguments(Prepare(dayz));
        Check(args.Count(a => a.StartsWith("-port=")) == 1 && args.Contains("-port=2402") && args.Count(a => a.StartsWith("-mission=")) == 1,
            "Managed launch arguments replace conflicting old parameters");
        Check(args.Single(a => a.StartsWith("-mod=")) == "-mod=@Manual;@Shared;@Selected", "Manual and Addon mods merge once in order without duplicates");
        Check(args.Contains("-serverMod=@Server Only") && args.Contains("-freezeCheck"), "Server-only mods and custom switches are preserved");
        Check(args.Contains("-profiles=" + Path.GetFullPath(Path.Combine(root, "profile with spaces\\"))), "Profile paths with spaces and trailing backslashes are passed correctly");

        string custom = Path.Combine(root, "custom config", "private server.cfg");
        Directory.CreateDirectory(Path.GetDirectoryName(custom));
        File.WriteAllText(custom, original);
        settings.ServerParam = "-config=\"custom config\\private server.cfg\" -profiles=profile";
        args = Arguments(Prepare(dayz));
        Check(args.Contains("-config=" + custom) && File.ReadAllText(custom).Contains("steamQueryPort= 2405;"), "Custom configuration paths are synchronized");
        settings.ServerParam = "-config=missing.cfg";
        Reject(dayz, "Missing custom configs block startup");
        settings.ServerParam = "-config=serverDZ.cfg -config=other.cfg";
        Reject(dayz, "Duplicate custom config options block startup");
        settings.ServerParam = "-profiles=\"unclosed";
        Reject(dayz, "Unclosed parameter quotes block startup");
        settings.ServerParam = "-config=serverDZ.cfg";

        foreach (string broken in new[] { "hostname=\"open;", "/* open", "class Missions {", "hostname=\"bad\"\nmaxPlayers=20;", "}" })
        {
            File.WriteAllText(path, broken);
            Reject(dayz, "Malformed DayZ config is rejected: " + broken.Replace("\n", " "));
            Check(File.ReadAllText(path) == broken, "Rejected configuration remains unchanged");
        }
        File.WriteAllText(path, original);
        File.SetAttributes(path, FileAttributes.ReadOnly);
        try { Reject(dayz, "A failed config write blocks startup"); Check(File.ReadAllText(path) == original && Directory.GetFiles(root, "*.tmp").Length == 0, "Failed writes preserve the original and clean up temporary files"); }
        finally { File.SetAttributes(path, FileAttributes.Normal); }
        settings.ServerMaxPlayer = "0"; Reject(dayz, "Invalid slots are rejected"); settings.ServerMaxPlayer = "42";
        settings.ServerPort = "70000"; Reject(dayz, "Out-of-range game ports are rejected"); settings.ServerPort = "2402";
        settings.ServerQueryPort = "2402"; Reject(dayz, "Colliding game and query ports are rejected"); settings.ServerQueryPort = "2405";
        settings.ServerIP = "127.0.0.999"; Reject(dayz, "Invalid IPv4 bind addresses are rejected"); settings.ServerIP = "0.0.0.0";
        settings.ServerMap = "..\\outside"; Reject(dayz, "Mission path traversal is rejected"); settings.ServerMap = "missing"; Reject(dayz, "Missing mission folders are rejected"); settings.ServerMap = "dayzOffline.chernarusplus";
        settings.ServerName = "bad\nname"; Reject(dayz, "Multiline server names are rejected"); settings.ServerName = "Second name";
        Directory.CreateDirectory(Path.Combine(root, "mpmissions", "dayzOffline.enoch"));
        settings.ServerMap = "dayzOffline.enoch";
        settings.ServerMaxPlayer = "70"; settings.ServerQueryPort = "2505";
        args = Arguments(Prepare(dayz));
        Check(args.Contains("-mission=" + Path.Combine(root, "mpmissions", "dayzOffline.enoch")) && File.ReadAllText(path).Contains("2505;"), "Changed map and query port take effect at the next start");
        Check(await dayz.Start() == null && dayz.Error.Contains("DayZServer_x64.exe"), "Missing game executable returns a useful startup error");
        Check(!dayz.IsInstallValid() && !dayz.IsImportValid(root), "Install and Import require the actual DayZ executable");
        File.WriteAllText(Path.Combine(root, "DZSALModServer.exe"), "fixture");
        Check(dayz.StartPath == "DZSALModServer.exe" && !dayz.IsInstallValid(), "DZSA is resolved before central firewall setup but cannot replace missing game files");
        Check(FirewallPaths(id, true).SequenceEqual(new[] { Path.Combine(root, "DayZServer_x64.exe"), Path.Combine(root, "DZSALModServer.exe") }), "Central firewall resolves both the game and launcher executables");
        File.WriteAllText(Path.Combine(root, "DayZServer_x64.exe"), "fixture");
        Check(dayz.IsInstallValid() && dayz.IsImportValid(root), "Installed DayZ files are recognized");
        File.Delete(Path.Combine(root, "DZSALModServer.exe"));
        Check(dayz.StartPath == "DayZServer_x64.exe", "Removing DZSA restores the native executable");
        Check(FirewallPaths(id, true).Length == 1 && FirewallPaths(id, false).Length == 2, "Disabled Auto Firewall also targets stale DZSA rules without adding missing launchers");
        Check(dayz.AppId == "223350" && !dayz.loginAnonymous && dayz.PortIncrements >= 4, "DayZ keeps account-based SteamCMD installation and separates instance ports");
        await dayz.Stop(null);

        var other = Settings(secondId);
        other.ServerName = "Other instance";
        string secondRoot = ServerPath.GetServersServerFiles(secondId);
        Directory.CreateDirectory(Path.Combine(secondRoot, "mpmissions", other.ServerMap));
        Prepare(create(other));
        Check(File.ReadAllText(Path.Combine(secondRoot, "serverDZ.cfg")).Contains("Other instance") && !File.ReadAllText(path).Contains("Other instance"), "DayZ instances keep separate configurations");
    }

    private static void CheckFirewallIntegration(string id)
    {
        Type planner = Fork.GetType("WindowsGSM.Functions.ServerFirewall", true);
        var programs = planner.GetMethod("GetProgramPaths", BindingFlags.NonPublic | BindingFlags.Static);
        string root = ServerPath.GetServersServerFiles(id);
        Directory.CreateDirectory(root);
        var games = new[] {
            new[] { "Farming Simulator 22 Dedicated Server", "dedicatedServer.exe", @"x64\FarmingSimulator2022Game.exe" },
            new[] { "Farming Simulator 25 Dedicated Server", "dedicatedServer.exe", @"x64\FarmingSimulator2025Game.exe" },
            new[] { "SCP: Secret Laboratory Dedicated Server", "LocalAdmin.exe", "SCPSL.exe" }
        };
        foreach (var game in games)
        {
            string child = Path.Combine(root, game[2]);
            Directory.CreateDirectory(Path.GetDirectoryName(child));
            File.WriteAllText(child, "fixture");
            var enabled = ((IEnumerable<string>)programs.Invoke(null, new object[] { id, game[0], game[1], true })).ToArray();
            Check(enabled.Contains(child) && enabled.Contains(Path.Combine(root, game[1])), game[0] + " firewall includes the launcher and game process");
            File.Delete(child);
            var disabled = ((IEnumerable<string>)programs.Invoke(null, new object[] { id, game[0], game[1], false })).ToArray();
            Check(disabled.Contains(child), game[0] + " disabled firewall also targets the removed child executable");
        }
        var settings = Settings(id);
        settings.ServerGame = "Minecraft: All the Mods 10 (ATM10) [ATM10.cs]";
        settings.ServerPort = "25565";
        settings.ServerQueryPort = "25566";
        var portMethod = planner.GetMethod("GetPortRules", BindingFlags.NonPublic | BindingFlags.Static);
        var rules = ((IEnumerable)portMethod.Invoke(null, new object[] { settings, true })).Cast<object>().ToArray();
        Func<object, string, object> field = (rule, name) => rule.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(rule);
        Check(rules.Length == 2 && (int)field(rules[0], "Protocol") == 6 && (string)field(rules[0], "LocalPorts") == "25565"
            && (int)field(rules[1], "Protocol") == 17 && (string)field(rules[1], "LocalPorts") == "25566", "ATM10 has separate TCP game and UDP query rules for the configured ports");
        var paths = ((IEnumerable<string>)programs.Invoke(null, new object[] { id, settings.ServerGame, "startserver.bat", true })).ToArray();
        Check(paths.Length == 0, "ATM10 does not authorize a batch file or a shared Java runtime globally");
        settings.ServerID = id + "-other";
        var other = ((IEnumerable)portMethod.Invoke(null, new object[] { settings, true })).Cast<object>().ToArray();
        Check((string)field(rules[0], "Name") != (string)field(other[0], "Name"), "ATM10 firewall names are separate for each server instance");
        settings.ServerQueryPort = "invalid";
        try { ((IEnumerable)portMethod.Invoke(null, new object[] { settings, true })).Cast<object>().ToArray(); throw new Exception("Invalid query port accepted"); }
        catch (InvalidOperationException) { Check(true, "Invalid ATM10 firewall ports are rejected before applying a rule"); }
        Check(((IEnumerable)portMethod.Invoke(null, new object[] { settings, false })).Cast<object>().Count() == 2, "Disabling ATM10 rules does not depend on valid saved ports");
        Type firewall = Fork.GetType("WindowsGSM.WindowsFirewall", true);
        dynamic detached = firewall.GetMethod("CreatePortRule", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { rules[0] });
        Check(detached.LocalPorts == "25565" && detached.Protocol == 6 && detached.Grouping == "WindowsGSM Auto Firewall"
            && detached.Direction == 1 && detached.Action == 1 && detached.Enabled, "Windows accepts the detached COM port-rule properties without adding it to the firewall");
        var policy = Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
        dynamic policyView = policy;
        object missing = firewall.GetMethod("FindPortRule", BindingFlags.NonPublic | BindingFlags.Static)
            .Invoke(null, new object[] { policyView.Rules, "WindowsGSM nonexistent check " + Guid.NewGuid().ToString("N") });
        Check(missing == null, "The firewall lookup handles a missing named rule without changing rules");
    }
    private static async Task CheckFailedSteamUpdate(string id)
    {
        var settings = Settings(id);
        dynamic server = Create("Arma 3 Dedicated Server", settings);
        var process = await server.Update();
        Check(process == null && ((string)server.Error).Contains("Steam account"), "A missing Steam login returns an update error without throwing");
    }
    private static Type CompileStandalone(string path)
    {
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator)
            .Concat(new[] { Fork.Location }).Distinct(StringComparer.OrdinalIgnoreCase).Select(p => MetadataReference.CreateFromFile(p));
        var syntax = CSharpSyntaxTree.ParseText(File.ReadAllText(path), new CSharpParseOptions(LanguageVersion.CSharp6));
        var compilation = CSharpCompilation.Create("DayZStandaloneChecks", new[] { syntax }, references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        using (var stream = new MemoryStream())
        {
            var result = compilation.Emit(stream);
            if (!result.Success) throw new Exception(string.Join(Environment.NewLine, result.Diagnostics));
            Check(result.Success, "Standalone DayZ compiles as C# 6 against the actual WindowsGSM API");
            Type type = Assembly.Load(stream.ToArray()).GetType("WindowsGSM.Plugins.DayZ", true);
            dynamic plugin = Activator.CreateInstance(type, new object[] { null });
            Check(plugin.FullName == "DayZ Dedicated Server" && plugin.Plugin.name == "WindowsGSM.DayZ", "Standalone metadata loads through the standard plugin contract");
            return type;
        }
    }

    private static int Main(string[] args)
    {
        string id = "dayz-check-" + Guid.NewGuid().ToString("N");
        string secondId = id + "-second";
        try
        {
            CheckCatalog();
            CheckFirewallIntegration(id);
            CheckFailedSteamUpdate(id).GetAwaiter().GetResult();
            CheckDayZ(id, secondId, data => Create("DayZ Dedicated Server", data)).GetAwaiter().GetResult();
            if (args.Length != 0)
            {
                Type standalone = CompileStandalone(args[0]);
                CheckDayZ(id + "-plugin", secondId + "-plugin", data => Activator.CreateInstance(standalone, data)).GetAwaiter().GetResult();
            }
            Console.WriteLine(passed + " checks passed. No game server or firewall rule was started or changed.");
            return 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
        finally
        {
            string safeRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "servers")) + Path.DirectorySeparatorChar;
            foreach (string item in new[] { id, secondId, id + "-plugin", secondId + "-plugin" })
            {
                string path = Path.GetFullPath(ServerPath.GetServers(item));
                if (path.StartsWith(safeRoot, StringComparison.OrdinalIgnoreCase) && Directory.Exists(path)) Directory.Delete(path, true);
            }
        }
    }
}
