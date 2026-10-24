// Made by PapaGordon
using System;
using WindowsGSM.Functions;

namespace WindowsGSM.GameServer.Data
{
    internal static class BuiltInServers
    {
        private sealed class Entry
        {
            internal readonly string Name, FileName;
            internal readonly Func<ServerConfig, object> Create;
            internal Entry(string name, string fileName, Func<ServerConfig, object> create)
            {
                Name = name;
                FileName = fileName;
                Create = create;
            }
        }

        private static readonly Entry[] Entries =
        {
            new Entry(DAYZ.FullName, "DayZ.cs", data => new DAYZ(data)),
            new Entry("Arma 3 Dedicated Server", "ARMA3.cs", data => new Bundled.ARMA3(data)),
            new Entry("Minecraft: All the Mods 10 (ATM10)", "ATM10.cs", data => new Bundled.ATM10(data)),
            new Entry("Enshrouded Dedicated Server", "Enshrouded.cs", data => new Bundled.Enshrouded(data)),
            new Entry("Farming Simulator 22 Dedicated Server", "FarmingSimulator22.cs", data => new Bundled.FarmingSimulator22(data)),
            new Entry("Farming Simulator 25 Dedicated Server", "FarmingSimulator25.cs", data => new Bundled.FarmingSimulator25(data)),
            new Entry("Project Zomboid Dedicated Server", "ProjectZomboid.cs", data => new Bundled.ProjectZomboid(data)),
            new Entry("SCP: Secret Laboratory Dedicated Server", "SCPSL.cs", data => new Bundled.SCPSL(data)),
            new Entry("Valheim Dedicated Server", "Valheim.cs", data => new Bundled.Valheim(data))
        };

        internal static string GetName(string name)
        {
            foreach (var entry in Entries)
                if (name == entry.Name || name == entry.Name + " [" + entry.FileName + "]") return entry.Name;
            return name;
        }

        internal static bool Contains(string name)
        {
            string canonical = GetName(name);
            foreach (var entry in Entries)
                if (canonical == entry.Name) return true;
            return false;
        }

        internal static object Get(string name, ServerConfig serverData)
        {
            string canonical = GetName(name);
            foreach (var entry in Entries)
                if (canonical == entry.Name) return entry.Create(serverData);
            return null;
        }
    }
}
