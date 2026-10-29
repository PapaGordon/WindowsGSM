// Made by PapaGordon
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;

internal static class WorkshopChecks
{
    private static int passed;
    private static string root;
    private static void Check(bool value, string text)
    {
        if (!value) throw new Exception(text);
        passed++; Console.WriteLine("PASS " + text);
    }
    private static void Fails(Action action, string text)
    {
        try { action(); } catch (Exception) { Check(true, text); return; }
        throw new Exception("Expected refusal: " + text);
    }
    private static void Write(string path, string value)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path, value);
    }
    private static List<WorkshopUpdater.Item> List(params string[] lines)
    {
        string config = Path.Combine(root, "parse.cfg");
        File.WriteAllLines(config, lines); return WorkshopUpdater.Read(config);
    }
    private static string Mod(string folder, string version, string key = "sign.bikey")
    {
        string path = Path.Combine(root, "sources", folder + "-" + Guid.NewGuid().ToString("N"));
        Write(Path.Combine(path, "addons", "fixture.pbo"), version);
        Write(Path.Combine(path, "keys", key), version);
        return path;
    }
    private static void Deploy(string live, string state, List<WorkshopUpdater.Item> items, params string[] sources)
    {
        Directory.CreateDirectory(live);
        WorkshopUpdater.Deploy(live, state, "221100", items, items.Select((item, i) => new { item.Id, Source = sources[i] }).ToDictionary(x => x.Id, x => x.Source));
    }
    private static async Task Run()
    {
        Check(WorkshopUpdater.Read(Path.Combine(root, "absent.cfg")).Count == 0, "No config keeps Workshop disabled");
        var items = List("# comment", "123|@Client_Mod|mod", "456|@Server|servermod");
        Check(items.Count == 2 && items[1].Mode == "servermod", "Explicit IDs and client/server roles preserve list order");
        foreach (string bad in new[] { "123|../world|mod", "123|@a b|mod", "12 +quit|@A|mod", "00123|@A|mod", "123|@A|other", "123|@A", "18446744073709551616|@A|mod" })
            Fails(() => List(bad), "Refuses malformed entry: " + bad);
        Fails(() => List("123|@A|mod", "123|@B|mod"), "Duplicate IDs are refused");
        Fails(() => List("123|@A|mod", "456|@a|mod"), "Duplicate target folders are refused case-insensitively");
        string args = WorkshopUpdater.MergeArguments("-config=server.cfg \"-mod=@Existing;@Client_Mod\" -serverMod=\"@Admin\" -profiles=\"my profile\"", items);
        Check(args.Contains("\"-mod=@Existing;@Client_Mod\"") && args.Contains("\"-servermod=@Admin;@Server\""), "Arguments retain manual mods and merge Workshop folders without duplicates");
        Check(args.Contains("-profiles=\"my profile\"") && args.Contains("-config=server.cfg"), "Unrelated quoted arguments survive unchanged");
        Check(WorkshopUpdater.MergeArguments("literal", new List<WorkshopUpdater.Item>()) == "literal", "Disabled updater leaves arguments exactly unchanged");

        string live = Path.Combine(root, "live"), state = Path.Combine(root, "state");
        var one = List("123|@Client_Mod|mod");
        Write(Path.Combine(live, "@Client_Mod", "old.pbo"), "old");
        Write(Path.Combine(live, "@Own", "own.pbo"), "custom");
        Write(Path.Combine(live, "keys", "manual.bikey"), "manual");
        string first = Mod("first", "v1");
        Deploy(live, state, one, first);
        Check(!File.Exists(Path.Combine(live, "@Client_Mod", "old.pbo")) && File.ReadAllText(Path.Combine(live, "@Client_Mod", "addons", "fixture.pbo")) == "v1", "Configured mod folders are replaced completely");
        Check(File.ReadAllText(Path.Combine(live, "@Own", "own.pbo")) == "custom" && File.ReadAllText(Path.Combine(live, "keys", "manual.bikey")) == "manual", "Unlisted mods and manual keys are preserved");
        Check(Directory.GetFiles(Path.Combine(live, ".wgsm-workshop-backups"), "old.pbo", SearchOption.AllDirectories).Length == 1, "The original configured folder remains in the backup");
        string oldKey = Path.Combine(live, "keys", "wgsm_221100_123_sign.bikey");
        Check(File.ReadAllText(oldKey) == "v1", "Signing keys are copied with a managed filename");
        string second = Mod("second", "v2", "new.bikey");
        Deploy(live, state, one, second);
        string newKey = Path.Combine(live, "keys", "wgsm_221100_123_new.bikey");
        Check(!File.Exists(oldKey) && File.ReadAllText(newKey) == "v2", "Only unchanged owned keys are rotated");
        File.WriteAllText(newKey, "user-change");
        Fails(() => Deploy(live, state, one, first), "A locally changed managed key blocks deployment");
        Check(File.ReadAllText(Path.Combine(live, "@Client_Mod", "addons", "fixture.pbo")) == "v2", "Key conflict leaves the live mod unchanged");
        File.WriteAllText(newKey, "v2");
        using (var locked = new FileStream(newKey, FileMode.Open, FileAccess.Read, FileShare.Read))
            Fails(() => Deploy(live, state, one, first), "A locked key triggers batch rollback after the folder swap");
        Check(File.ReadAllText(Path.Combine(live, "@Client_Mod", "addons", "fixture.pbo")) == "v2" && !File.Exists(Path.Combine(state, "pending.txt")), "Rollback restores the previous folder and clears recovery state");
        string empty = Path.Combine(root, "empty"); Directory.CreateDirectory(empty);
        Fails(() => Deploy(live, state, one, empty), "Empty or non-server Workshop items are refused");
        Write(Path.Combine(state, "pending.txt"), "simulated interruption");
        Fails(() => Deploy(live, state, one, first), "Interrupted deployments require recovery before another activation");
        File.Delete(Path.Combine(state, "pending.txt"));

        string config = Path.Combine(root, "update-config"), steam = ServerPath.GetBin("steamcmd");
        if (!Path.GetFullPath(steam).StartsWith(Path.GetFullPath(AppContext.BaseDirectory), StringComparison.OrdinalIgnoreCase)) throw new Exception("Unsafe test SteamCMD path");
        if (Directory.Exists(steam)) throw new Exception("Refusing to replace existing test SteamCMD data");
        Write(Path.Combine(steam, "steamcmd.exe"), "fixture - never executed");
        string encrypted = Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes("synthetic-pass"), null, DataProtectionScope.CurrentUser));
        Write(Path.Combine(steam, "userData.txt"), "steamUser=fixture\nsteamPassEncrypted=" + encrypted);
        Write(Path.Combine(config, "WorkshopMods.cfg"), "123|@Client_Mod|mod");
        string parameters = WorkshopUpdater.Parameters("C:\\fixture", "221100", one);
        Check(parameters.Contains("+login \"fixture\" \"synthetic-pass\"") && parameters.Contains("+workshop_download_item 221100 123 validate") && !parameters.Contains("+app_update"), "Workshop commands use the host's encrypted login and client app ID");
        Check(WorkshopUpdater.Parameters("C:\\fixture", "107410", one).Contains("+workshop_download_item 107410 123 validate"), "Arma Workshop uses app 107410");
        string cached = Path.Combine(config, "workshop-state-221100", "download", "steamapps", "workshop", "content", "221100", "123");
        Write(Path.Combine(cached, "addons", "fixture.pbo"), "downloaded");
        Func<string, string, Task<Tuple<int, string>>> ok = (exe, command) => Task.FromResult(Tuple.Create(0, "Success. Downloaded item 123 to \"" + cached + "\" (10 bytes)"));
        await WorkshopUpdater.Update(live, config, steam, "221100", ok);
        Check(File.ReadAllText(Path.Combine(live, "@Client_Mod", "addons", "fixture.pbo")) == "downloaded", "Confirmed downloads deploy from the selected cache");
        Fails(() => WorkshopUpdater.Update(live, config, steam, "221100", (a, b) => Task.FromResult(Tuple.Create(0, "ERROR! Download failed"))).GetAwaiter().GetResult(), "Exit zero without per-item confirmation is refused");
        Fails(() => WorkshopUpdater.Update(live, config, steam, "221100", (a, b) => Task.FromResult(Tuple.Create(1, "failed"))).GetAwaiter().GetResult(), "Nonzero SteamCMD exit blocks startup");
        Fails(() => WorkshopUpdater.Update(live, config, steam, "221100", (a, b) => Task.FromResult(Tuple.Create(0, "Success. Downloaded item 123 to C:\\unrelated"))).GetAwaiter().GetResult(), "Stale cache without a matching receipt path is refused");
        Write(Path.Combine(config, "WorkshopMods.cfg"), "123|@Client_Mod|mod\n456|@Other|mod");
        Fails(() => WorkshopUpdater.Update(live, config, steam, "221100", ok).GetAwaiter().GetResult(), "Missing confirmation for one item blocks the whole batch");
        Check(File.ReadAllText(Path.Combine(live, "@Client_Mod", "addons", "fixture.pbo")) == "downloaded", "Failed batches leave existing live mods unchanged");
        File.WriteAllText(Path.Combine(steam, "userData.txt"), "steamUser=fixture\nsteamPass=plain\nsteamPassEncrypted=broken!");
        Fails(() => WorkshopUpdater.Parameters("C:\\fixture", "221100", one), "Broken encryption never falls back to plaintext for Workshop downloads");
        Directory.Delete(steam, true);

        foreach (string game in new[] { "DayZ", "Arma" })
        {
            var settings = new ServerConfig("workshop-start-" + game)
            {
                ServerPort = "2302", ServerQueryPort = "2305", ServerMap = "dayzOffline.chernarusplus", ServerMaxPlayer = "60",
                ServerName = "Workshop fixture", ServerIP = "127.0.0.1", ServerParam = ""
            };
            string files = ServerPath.GetServersServerFiles(settings.ServerID);
            string cfg = ServerPath.GetServersConfigs(settings.ServerID, "WorkshopMods.cfg");
            Write(cfg, "invalid fixture entry");
            Directory.CreateDirectory(Path.Combine(files, "mpmissions", settings.ServerMap));
            if (game == "DayZ")
            {
                Write(Path.Combine(files, "DayZServer_x64.exe"), "never execute");
                var server = new WindowsGSM.GameServer.DAYZ(settings);
                Check(await server.Start() == null && server.Error.Contains("Invalid Workshop entry"), "DayZ cancels the actual Start path on invalid Workshop configuration");
            }
            else
            {
                Write(Path.Combine(files, "arma3server_x64.exe"), "never execute");
                var server = new WindowsGSM.GameServer.Bundled.ARMA3(settings);
                Check(await server.Start() == null && server.Error.Contains("Invalid Workshop entry"), "Arma cancels the actual Start path on invalid Workshop configuration");
            }
            Directory.Delete(ServerPath.GetServers(settings.ServerID), true);
        }
    }
    private static int Main()
    {
        root = Path.Combine(AppContext.BaseDirectory, "workshop-fixture-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try { Run().GetAwaiter().GetResult(); Console.WriteLine(passed + " Workshop checks passed. No real Steam login or server started."); return 0; }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }
}
