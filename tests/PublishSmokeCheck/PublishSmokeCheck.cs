// Made by PapaGordon
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

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
            string[] names = { "DayZ Dedicated Server", "Arma 3 Dedicated Server", "Minecraft: All the Mods 10 (ATM10)",
                "Enshrouded Dedicated Server", "Farming Simulator 22 Dedicated Server", "Farming Simulator 25 Dedicated Server",
                "Project Zomboid Dedicated Server", "SCP: Secret Laboratory Dedicated Server", "Valheim Dedicated Server" };
            foreach (string name in names)
                if (factory.Invoke(null, new object[] { name, null, null }) == null) throw new Exception("Missing built-in server: " + name);
            Console.WriteLine("PASS All nine built-in entries load from the published EXE.");
            Type manager = app.GetType("WindowsGSM.Functions.PluginManagement", true);
            object instance = Activator.CreateInstance(manager);
            string plugin = Path.Combine(root, "plugins", "DayZ.cs", "DayZ.cs");
            Task task = (Task)manager.GetMethod("LoadPlugin").Invoke(instance, new object[] { plugin, false });
            task.GetAwaiter().GetResult();
            object metadata = task.GetType().GetProperty("Result").GetValue(task);
            bool loaded = (bool)metadata.GetType().GetField("IsLoaded").GetValue(metadata);
            if (!loaded) throw new Exception("DayZ plugin did not load: " + metadata.GetType().GetField("Error").GetValue(metadata));
            Console.WriteLine("PASS The published EXE compiles and loads the standalone DayZ plugin through its real plugin loader.");
            Console.WriteLine("Published EXE smoke check passed. No UI or game server was started.");
            Environment.Exit(0);
        }
        catch (Exception e) { Console.Error.WriteLine(e); Environment.Exit(1); }
    }
}
