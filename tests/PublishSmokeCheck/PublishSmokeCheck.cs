// Made by PapaGordon
using System;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

internal static class StartupHook
{
    private static void CheckGameIcons(Assembly app)
    {
        var artwork = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        using (var stream = app.GetManifestResourceStream("WindowsGSM.g.resources"))
        using (var resources = new ResourceReader(stream))
        {
            foreach (DictionaryEntry resource in resources)
            {
                string path = (string)resource.Key;
                if (!path.StartsWith("images/games/", StringComparison.OrdinalIgnoreCase)) continue;
                using (var copy = new MemoryStream())
                {
                    ((Stream)resource.Value).CopyTo(copy);
                    artwork.Add(path, copy.ToArray());
                }
            }
        }
        string manifest = Environment.GetEnvironmentVariable("WGSM_ICON_MANIFEST");
        if (!string.IsNullOrEmpty(manifest))
            foreach (string line in File.ReadLines(manifest))
            {
                string[] item = line.Split('\t');
                if (item.Length != 2 || !artwork.TryGetValue(item[0], out var bytes)
                    || Convert.ToHexString(SHA256.HashData(bytes)) != item[1])
                    throw new Exception("Packaged icon differs from the checked source: " + item[0]);
            }
        int count = 0;
        using (var stream = app.GetManifestResourceStream("WindowsGSM.GameServer.Data.Icon.resources"))
        using (var mappings = new ResourceReader(stream))
        {
            foreach (DictionaryEntry mapping in mappings)
            {
                string path = (string)mapping.Value;
                if (!artwork.TryGetValue(path, out var bytes)) throw new Exception("Missing game icon: " + mapping.Key);
                using (var image = new MemoryStream(bytes))
                {
                    var decoder = BitmapDecoder.Create(image, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    if (decoder.Frames.Count == 0 || decoder.Frames[0].PixelWidth == 0) throw new Exception("Invalid game icon: " + mapping.Key);
                }
                count++;
            }
        }
        Console.WriteLine("PASS All " + count + " game-icon mappings decode from the final EXE; " + artwork.Count + " embedded game images checked.");
    }

    private static void CheckAtmUpdate(Assembly app, string root)
    {
        string fixture = Path.Combine(root, "atm-update-check-" + Guid.NewGuid().ToString("N"));
        string source = Path.Combine(fixture, "pack"), previous = Path.Combine(fixture, "previous"), stage = Path.Combine(fixture, "stage");
        string older = "cc-tweaked-1.21.1-forge-1.113.1.jar", newer = "cc-tweaked-1.21.1-forge-1.120.2.jar";
        try
        {
            foreach (string directory in new[] { source, previous, stage }) Directory.CreateDirectory(Path.Combine(directory, "mods"));
            foreach (string jar in new[] { Path.Combine(previous, "mods", older), Path.Combine(source, "mods", newer) })
            using (var zip = ZipFile.Open(jar, ZipArchiveMode.Create))
            using (var writer = new StreamWriter(zip.CreateEntry("META-INF/neoforge.mods.toml").Open()))
                writer.Write("[[mods]]\nmodId=\"computercraft\"\nversion=\"fixture\"\n");
            File.Copy(Path.Combine(previous, "mods", older), Path.Combine(stage, "mods", older));
            File.WriteAllText(Path.Combine(stage, "mods", "MyExtra.jar"), "separate additional mod");
            Directory.CreateDirectory(Path.Combine(previous, ".wgsm-atm10-config-defaults"));
            File.WriteAllText(Path.Combine(previous, ".wgsm-atm10-config-defaults", "pack-mods.sha256"), "version\t8.3\n");
            Type type = app.GetType("WindowsGSM.GameServer.Bundled.ATM10", true);
            object config = Activator.CreateInstance(app.GetType("WindowsGSM.Functions.ServerConfig", true), "packed-atm-fixture");
            object server = Activator.CreateInstance(type, config);
            type.GetMethod("MergePackMods", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(server, new object[] { source, previous, stage });
            if (File.Exists(Path.Combine(stage, "mods", older)) || !File.Exists(Path.Combine(stage, "mods", newer))
                || !File.Exists(Path.Combine(stage, "mods", "MyExtra.jar")) || !File.Exists(Path.Combine(previous, "mods", older)))
                throw new Exception("Published ATM10 does not replace the old CC:Tweaked JAR while preserving additional/live files.");
            Console.WriteLine("PASS Published WGSM " + app.GetName().Version + " replaces legacy CC:Tweaked 1.113.1 with 1.120.2 and keeps additional mods.");
            File.Copy(Path.Combine(previous, "mods", older), Path.Combine(source, "mods", older));
            type.GetMethod("MergePackMods", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(server, new object[] { source, previous, stage });
            if (!File.Exists(Path.Combine(stage, "mods", older)) || !File.Exists(Path.Combine(stage, "mods", newer)))
                throw new Exception("A pack-supplied CC:Tweaked candidate was removed.");
            Console.WriteLine("PASS Published ATM10 preserves both pack-supplied CC:Tweaked candidates for NeoForge discovery.");
            string relative = "config/neovitae/materials.json", oldMaterials = "[{\"name\":\"iron\"},{\"name\":\"titanium\"}]", newMaterials = "[{\"name\":\"iron\"}]";
            foreach (string directory in new[] { source, previous, stage }) Directory.CreateDirectory(Path.Combine(directory, "config", "neovitae"));
            File.WriteAllText(Path.Combine(source, relative), newMaterials);
            File.WriteAllText(Path.Combine(previous, relative), oldMaterials);
            File.WriteAllText(Path.Combine(stage, relative), oldMaterials);
            File.WriteAllText(Path.Combine(stage, "config", "other.cfg"), "keep preferences");
            File.WriteAllText(Path.Combine(source, "startserver.bat"), "fixture launcher");
            string archive = Path.Combine(fixture, "ServerFiles-8.3.zip");
            ZipFile.CreateFromDirectory(source, archive);
            var current = type.GetMethod("IsPackCurrent", BindingFlags.Instance | BindingFlags.NonPublic);
            if ((bool)current.Invoke(server, new object[] { stage, archive, "8.3", "8.3" })) throw new Exception("Published EXE skips same-version registry repair.");
            string backup = Path.Combine(fixture, "backup"); Directory.CreateDirectory(backup);
            var conflicts = new List<string> { relative + " (existing file kept)" };
            type.GetMethod("SynchronizePackRegistries", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(server, new object[] { source, stage, backup, conflicts });
            if (File.ReadAllText(Path.Combine(stage, relative)) != newMaterials || File.ReadAllText(Path.Combine(previous, relative)) != oldMaterials
                || File.ReadAllText(Path.Combine(stage, "config", "other.cfg")) != "keep preferences" || conflicts.Count != 0)
                throw new Exception("Published EXE does not synchronize only the pack registry definition.");
            Console.WriteLine("PASS Published ATM10 synchronizes the material registry while keeping previous files and other preferences.");
            if (!(bool)current.Invoke(server, new object[] { stage, archive, "8.3", "8.3" })) throw new Exception("Matching registries trigger needless reinstall.");
            Console.WriteLine("PASS Published ATM10 repairs same-version registry differences and skips an already matching pack.");
        }
        finally { if (Directory.Exists(fixture)) Directory.Delete(fixture, true); }
    }

    public static void Initialize()
    {
        try
        {
            Assembly app = Assembly.GetEntryAssembly();
            if (app.GetName().Name != "WindowsGSM") throw new Exception("Wrong entry assembly.");
            string root = Path.GetDirectoryName(Environment.ProcessPath);
            CheckGameIcons(app);
            CheckAtmUpdate(app, root);
            Directory.CreateDirectory(Path.Combine(root, "logs"));
            var factory = app.GetType("WindowsGSM.GameServer.Data.Class", true).GetMethod("Get", BindingFlags.Public | BindingFlags.Static);
            string[] names = { "DayZ Dedicated Server", "Minecraft: Java Edition Server", "Minecraft: NeoForge", "Arma 3 Dedicated Server", "Minecraft: All the Mods 10 (ATM10)",
                "Enshrouded Dedicated Server", "Farming Simulator 22 Dedicated Server", "Farming Simulator 25 Dedicated Server",
                "Project Zomboid Dedicated Server", "SCP: Secret Laboratory Dedicated Server", "Valheim Dedicated Server" };
            foreach (string name in names)
                if (factory.Invoke(null, new object[] { name, null, null }) == null) throw new Exception("Missing built-in server: " + name);
            Console.WriteLine("PASS All " + names.Length + " built-in entries load from the published EXE.");
            using (var stream = app.GetManifestResourceStream("WindowsGSM.g.resources"))
            using (var resources = new ResourceReader(stream))
            {
                bool found = false;
                foreach (DictionaryEntry resource in resources)
                {
                    if ((string)resource.Key != "images/games/neoforged.ico") continue;
                    var decoder = new IconBitmapDecoder((Stream)resource.Value, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    if (decoder.Frames.Count == 0 || decoder.Frames[0].PixelWidth != 128) throw new Exception("Invalid NeoForge icon.");
                    found = true;
                }
                if (!found) throw new Exception("NeoForge icon is missing from the EXE.");
            }
            if (app.GetManifestResourceStream("WindowsGSM.Licenses.NeoForge-art-attribution.txt") == null)
                throw new Exception("NeoForge artwork credit is missing.");
            Console.WriteLine("PASS The official NeoForge icon decodes through WPF and its artwork credit is embedded.");
            Type manager = app.GetType("WindowsGSM.Functions.PluginManagement", true);
            object instance = Activator.CreateInstance(manager);
            foreach (string file in new[] { "DayZ", "ARMA3", "ATM10", "Minecraft_Java", "Minecraft_NeoForge" })
            {
                string plugin = Path.Combine(root, "plugins", file + ".cs", file + ".cs");
                Task task = (Task)manager.GetMethod("LoadPlugin").Invoke(instance, new object[] { plugin, false });
                task.GetAwaiter().GetResult();
                object metadata = task.GetType().GetProperty("Result").GetValue(task);
                bool loaded = (bool)metadata.GetType().GetField("IsLoaded").GetValue(metadata);
                if (!loaded) throw new Exception(file + " plugin did not load: " + metadata.GetType().GetField("Error").GetValue(metadata));
                Console.WriteLine("PASS The published EXE compiles and loads " + file + " through its real plugin loader.");
            }
            Console.WriteLine("Published EXE smoke check passed. No UI or game server was started.");
            Environment.Exit(0);
        }
        catch (Exception e) { Console.Error.WriteLine(e); Environment.Exit(1); }
    }
}
