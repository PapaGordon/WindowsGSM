// Made by PapaGordon
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        internal static async Task<bool> Synchronize(ServerConfig settings, string startPath,
            Func<string, bool, Task<bool>> setProgram, Func<PortRule, bool, Task<bool>> setPort)
        {
            // Use one switch value for the complete operation, including all child processes.
            bool enabled = settings.AutoFirewall;
            bool success = true;
            foreach (string path in GetProgramPaths(settings.ServerID, settings.ServerGame, startPath, enabled))
                if (!await setProgram(path, enabled)) success = false;
            foreach (var rule in GetPortRules(settings, enabled))
                if (!await setPort(rule, enabled)) success = false;
            return success;
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
                case MC.FullName:
                    // Java servers use scoped port rules, never a global Java exception.
                    if (!enabled) yield return ServerPath.GetServersServerFiles(serverId, "server.jar");
                    yield break;
                case GameServer.Bundled.NeoForge.ServerName:
                    yield break;
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
            string game = BuiltInServers.GetName(settings.ServerGame);
            bool vanilla = game == MC.FullName;
            bool neoforge = game == GameServer.Bundled.NeoForge.ServerName;
            if (!vanilla && !neoforge && game != "Minecraft: All the Mods 10 (ATM10)") yield break;
            string scope;
            using (var hash = System.Security.Cryptography.SHA256.Create())
                scope = System.BitConverter.ToString(hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(
                    Path.GetFullPath(ServerPath.GetServersServerFiles(settings.ServerID)).ToUpperInvariant())))
                    .Replace("-", string.Empty).Substring(0, 12);
            string rulePrefix = "WindowsGSM " + scope + " Server " + settings.ServerID;
            string label = vanilla ? "Minecraft" : neoforge ? "NeoForge" : "ATM10";
            string gamePort = enabled ? ParsePort(settings.ServerPort, "Server Port") : string.Empty;
            string queryPort = enabled ? ParsePort(settings.ServerQueryPort, "Server Query Port") : string.Empty;
            yield return new PortRule { Name = rulePrefix + " - " + label + " Game", Protocol = 6,
                LocalPorts = gamePort };
            yield return new PortRule { Name = rulePrefix + " - " + label + " Query", Protocol = 17,
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
