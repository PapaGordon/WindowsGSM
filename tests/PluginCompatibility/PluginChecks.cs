// Made by PapaGordon
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using WindowsGSM.Functions;
using WindowsGSM.Installer;
using WindowsGSM.GameServer.Engine;

public class UpdateVersionFixture
{
    public string Version;
    public string GetLocalBuild() { return Version; }
}

internal static class PluginChecks
{
    private static int passed;
    private static void Check(bool value, string text)
    {
        if (!value) throw new Exception(text);
        passed++;
        Console.WriteLine("PASS " + text);
    }

    private static Assembly Compile(string source, string label)
    {
        var refs = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator)
            .Concat(new[] { typeof(ServerConfig).Assembly.Location }).Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(path => MetadataReference.CreateFromFile(path));
        var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(source), new CSharpParseOptions(LanguageVersion.Latest));
        var compilation = CSharpCompilation.Create("PluginCheck_" + Guid.NewGuid().ToString("N"), new[] { tree }, refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        using (var stream = new MemoryStream())
        {
            var result = compilation.Emit(stream);
            if (!result.Success) throw new Exception(label + ": " + string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));
            Check(true, label + " compiles against the real WGSM plugin API");
            return Assembly.Load(stream.ToArray());
        }
    }

    private static async Task CheckSteamAgent(object server, string label, string file)
    {
        var agent = server as SteamCMDAgent;
        if (agent == null) { Check(true, label + " does not use SteamCMD credentials"); return; }
        if (agent.loginAnonymous)
        {
            string args = SteamCMD.GetParameter("fixture", agent.AppId, loginAnonymous: true);
            Check(args.Contains("+login anonymous") && !args.Contains("fixture-pass"), label + " retains anonymous downloads without reading the password");
            return;
        }
        string encrypted = Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes("fixture-pass=123"), null, DataProtectionScope.CurrentUser));
        File.WriteAllText(file, "steamUser=\"fixture-user\"\nsteamPass=\"\"\nsteamPassEncrypted=\"" + encrypted + "\"\n");
        string parameters = SteamCMD.GetParameter("fixture", agent.AppId, loginAnonymous: false);
        Check(parameters.Contains("+login \"fixture-user\" \"fixture-pass=123\""), label + " uses encrypted credentials through the current installer path");
        var legacy = new SteamCMD();
        legacy.SetParameter("fixture", "", agent.AppId, true, false);
        string oldArgs = (string)typeof(SteamCMD).GetField("_param", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(legacy);
        Check(oldArgs.Contains("+login \"fixture-user\" \"fixture-pass=123\""), label + " uses encrypted credentials through the legacy installation path");
        File.WriteAllText(file, "steamUser=fixture-user\nsteamPass=fixture-plaintext-fallback\nsteamPassEncrypted=broken!\n");
        Process install = await agent.Install();
        Check(install == null && agent.Error != null, label + " refuses invalid encrypted data before starting SteamCMD during installation");
        var update = (Task<Process>)server.GetType().GetMethod("Update").Invoke(server, new object[] { false, null });
        Check(await update == null && agent.Error != null, label + " refuses invalid encrypted data before starting SteamCMD during update");
    }

    private static async Task Run(string manifestPath, string neoPath)
    {
        var probe = typeof(ServerConfig).Assembly.GetType("WindowsGSM.Functions.ServerUpdateVersion", true)
            .GetMethod("InstalledOrReported", BindingFlags.Static | BindingFlags.NonPublic);
        var version = new UpdateVersionFixture { Version = "8.1" };
        Check((string)probe.Invoke(null, new object[] { version, "8.1" }) == "8.1", "Version reporting reads the installed version before update");
        version.Version = "8.3";
        Check((string)probe.Invoke(null, new object[] { version, "8.1" }) == "8.3", "Success reporting refreshes installed 8.3 instead of repeating pre-update 8.1");
        version.Version = "unknown";
        Check((string)probe.Invoke(null, new object[] { version, "8.3" }) == "8.3", "Unavailable local probes retain the reported version");
        string bin = ServerPath.GetBin("steamcmd");
        string safe = Path.GetFullPath(AppContext.BaseDirectory).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!Path.GetFullPath(bin).StartsWith(safe, StringComparison.OrdinalIgnoreCase)) throw new Exception("Unsafe test path");
        string file = Path.Combine(bin, "userData.txt"), exe = Path.Combine(bin, "steamcmd.exe");
        if (File.Exists(file) || File.Exists(exe)) throw new Exception("The test folder already contains SteamCMD data; refusing to overwrite it.");
        Directory.CreateDirectory(bin);
        File.WriteAllText(exe, "fixture only - must never execute");
        try
        {
            var settings = new ServerConfig("compatibility-fixture");
            using (var manifest = JsonDocument.Parse(File.ReadAllText(manifestPath)))
            foreach (var row in manifest.RootElement.EnumerateArray())
            {
                string label = row.GetProperty("Repo").GetString();
                string source = row.GetProperty("Source")[0].GetString();
                Assembly plugin = Compile(source, label);
                Type type = plugin.GetExportedTypes().Single(t => t.Namespace == "WindowsGSM.Plugins" && t.GetConstructor(new[] { typeof(ServerConfig) }) != null);
                object instance = Activator.CreateInstance(type, settings);
                await CheckSteamAgent(instance, label, file);
            }
            Assembly neo = Compile(neoPath, "Standalone NeoForge");
            object neoforge = Activator.CreateInstance(neo.GetType("WindowsGSM.Plugins.Minecraft_NeoForge", true), settings);
            Check(!(neoforge is SteamCMDAgent), "Standalone NeoForge needs no Steam login");

            var factory = typeof(ServerConfig).Assembly.GetType("WindowsGSM.GameServer.Data.Class", true).GetMethod("Get");
            foreach (string name in new[] { "Arma 3 Dedicated Server", "DayZ Dedicated Server", "Farming Simulator 22 Dedicated Server", "Farming Simulator 25 Dedicated Server",
                "Enshrouded Dedicated Server", "Project Zomboid Dedicated Server", "SCP: Secret Laboratory Dedicated Server", "Valheim Dedicated Server" })
                await CheckSteamAgent(factory.Invoke(null, new object[] { name, settings, null }), "Built-in " + name, file);
            object canonical = factory.Invoke(null, new object[] { "Minecraft: NeoForge", settings, null });
            foreach (string suffix in new[] { "NeoForge.cs", "Minecraft_NeoForge.cs" })
                Check(factory.Invoke(null, new object[] { "Minecraft: NeoForge [" + suffix + "]", settings, null }).GetType() == canonical.GetType(), "NeoForge identifier " + suffix + " resolves to the built-in module");
            string sdkPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(manifestPath)), "../WindowsGSM-Plugin-Development/Plugins/DayZAuto.cs"));
            var sdk = Compile(sdkPath, "DayZAuto SDK example");
            object dayzAuto = Activator.CreateInstance(sdk.GetType("WindowsGSM.Plugins.DayZAuto", true), settings);
            string encrypted = Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes("fixture-pass=123"), null, DataProtectionScope.CurrentUser));
            File.WriteAllText(file, "steamUser=fixture-user\nsteamPassEncrypted=" + encrypted);
            var getLogin = dayzAuto.GetType().GetMethod("GetLogin", BindingFlags.Instance | BindingFlags.NonPublic);
            Check(((string)getLogin.Invoke(dayzAuto, null)).Contains("fixture-pass=123"), "DayZAuto SDK example reads encrypted credentials through the shared reader");
            File.WriteAllText(file, "steamUser=fixture-user\nsteamPass=fixture-fallback\nsteamPassEncrypted=broken!");
            Check(getLogin.Invoke(dayzAuto, null) == null, "DayZAuto SDK example rejects broken encrypted data without plaintext fallback");
            dayzAuto.GetType().GetMethod("DownloadMods", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(dayzAuto, new object[] { new Dictionary<string, string> { { "1559212036", "@Fixture" } } });
            Check(((SteamCMDAgent)dayzAuto).Error != null, "DayZAuto does not start a Workshop download after a credential error");
        }
        finally { File.Delete(file); File.Delete(exe); }
    }

    private static int Main(string[] args)
    {
        try { Run(args[0], args[1]).GetAwaiter().GetResult(); Console.WriteLine(passed + " plugin and encrypted-login compatibility checks passed. No Steam login or game server started."); return 0; }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
