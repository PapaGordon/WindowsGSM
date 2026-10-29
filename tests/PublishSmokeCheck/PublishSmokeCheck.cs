// Made by PapaGordon
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

internal static class StartupHook
{
    public static void Initialize()
    {
        try
        {
            Assembly app = Assembly.GetEntryAssembly();
            if (app.GetName().Name != "WindowsGSM") throw new Exception("Wrong entry assembly.");
            string root = Path.GetDirectoryName(Environment.ProcessPath);
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
