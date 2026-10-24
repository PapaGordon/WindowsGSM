// Made by PapaGordon
using System.Collections.Generic;
using System.IO;
using WindowsGSM.GameServer;
using WindowsGSM.GameServer.Data;

namespace WindowsGSM.Functions
{
    internal static class ServerFirewall
    {
        internal sealed class PortRule
        {
            internal string Name, LocalPorts;
            internal int Protocol;
        }

        internal static IEnumerable<string> GetProgramPaths(string serverId, string game, string startPath, bool enabled)
        {
            string[] programs;
            switch (BuiltInServers.GetName(game))
            {
                case DAYZ.FullName: programs = new[] { "DayZServer_x64.exe", "DZSALModServer.exe" }; break;
                case "Farming Simulator 22 Dedicated Server": programs = new[] { "dedicatedServer.exe", @"x64\FarmingSimulator2022Game.exe" }; break;
                case "Farming Simulator 25 Dedicated Server": programs = new[] { "dedicatedServer.exe", @"x64\FarmingSimulator2025Game.exe" }; break;
                case "SCP: Secret Laboratory Dedicated Server": programs = new[] { "LocalAdmin.exe", "SCPSL.exe" }; break;
                case "Minecraft: All the Mods 10 (ATM10)":
                    // Remove an old batch-file rule when disabling. Java uses the port rules below.
                    if (!enabled) yield return ServerPath.GetServersServerFiles(serverId, "startserver.bat");
                    yield break;
                default:
                    if (!string.IsNullOrWhiteSpace(startPath)) yield return ServerPath.GetServersServerFiles(serverId, startPath);
                    yield break;
            }
            for (int i = 0; i < programs.Length; i++)
            {
                string path = ServerPath.GetServersServerFiles(serverId, programs[i]);
                if (i == 0 || !enabled || File.Exists(path)) yield return path;
            }
        }

        internal static IEnumerable<PortRule> GetPortRules(ServerConfig settings, bool enabled)
        {
            if (BuiltInServers.GetName(settings.ServerGame) != "Minecraft: All the Mods 10 (ATM10)") yield break;
            string scope;
            using (var hash = System.Security.Cryptography.SHA256.Create())
                scope = System.BitConverter.ToString(hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(
                    Path.GetFullPath(ServerPath.GetServersServerFiles(settings.ServerID)).ToUpperInvariant())))
                    .Replace("-", string.Empty).Substring(0, 12);
            string rulePrefix = "WindowsGSM " + scope + " Server " + settings.ServerID;
            string gamePort = enabled ? ParsePort(settings.ServerPort, "Server Port") : string.Empty;
            string queryPort = enabled ? ParsePort(settings.ServerQueryPort, "Server Query Port") : string.Empty;
            yield return new PortRule { Name = rulePrefix + " - ATM10 Game", Protocol = 6,
                LocalPorts = gamePort };
            yield return new PortRule { Name = rulePrefix + " - ATM10 Query", Protocol = 17,
                LocalPorts = queryPort };
        }

        private static string ParsePort(string value, string field)
        {
            ushort port;
            if (!ushort.TryParse(value, out port) || port == 0)
                throw new System.InvalidOperationException(field + " must be between 1 and 65535 for Auto Firewall.");
            return port.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
