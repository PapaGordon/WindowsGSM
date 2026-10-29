// Made by PapaGordon
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WindowsGSM.GameServer.Engine
{
    internal static class WorkshopUpdater
    {
        internal sealed class Item
        {
            internal string Id, Folder, Mode;
        }

        internal static List<Item> Read(string path)
        {
            var items = new List<Item>();
            if (!File.Exists(path)) return items;
            foreach (string raw in File.ReadLines(path))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith("//")) continue;
                string[] parts = line.Split('|').Select(p => p.Trim()).ToArray();
                ulong id;
                if (parts.Length != 3 || !Regex.IsMatch(parts[0], @"\A[1-9][0-9]*\z")
                    || !ulong.TryParse(parts[0], out id)
                    || !Regex.IsMatch(parts[1], @"\A@[A-Za-z0-9_-]+\z")
                    || (parts[2] != "mod" && parts[2] != "servermod"))
                    throw new InvalidOperationException("Invalid Workshop entry. Use itemId|@Folder|mod or itemId|@Folder|servermod.");
                if (items.Any(i => i.Id == parts[0] || string.Equals(i.Folder, parts[1], StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Duplicate Workshop ID or folder: " + parts[0]);
                items.Add(new Item { Id = parts[0], Folder = parts[1], Mode = parts[2] });
            }
            return items;
        }

        internal static string MergeArguments(string arguments, List<Item> items)
        {
            if (items.Count == 0) return arguments;
            var tokens = Regex.Matches(arguments ?? "", "(?:\"[^\"]*\"|[^\\s\"]+)+").Cast<Match>().Select(m => m.Value).ToList();
            foreach (string mode in new[] { "mod", "servermod" })
            {
                var added = items.Where(i => i.Mode == mode).Select(i => i.Folder).ToList();
                if (added.Count == 0) continue;
                var folders = new List<string>();
                var keep = new List<string>();
                foreach (string token in tokens)
                {
                    string clean = token.Replace("\"", "");
                    if (clean.StartsWith("-" + mode + "=", StringComparison.OrdinalIgnoreCase))
                        folders.AddRange(clean.Substring(mode.Length + 2).Split(';').Where(p => !string.IsNullOrWhiteSpace(p)));
                    else keep.Add(token);
                }
                folders.AddRange(added);
                keep.Add("\"-" + mode + "=" + string.Join(";", folders.Distinct(StringComparer.OrdinalIgnoreCase)) + "\"");
                tokens = keep;
            }
            return string.Join(" ", tokens);
        }

        internal static string Parameters(string root, string appId, List<Item> items)
        {
            string parameters = WindowsGSM.Installer.SteamCMD.GetParameter(root, appId, false, false);
            string suffix = " +app_update " + appId + " +quit";
            if (parameters == null)
                throw new InvalidOperationException("Workshop login is unavailable. Check Steam credentials and the Windows account used to encrypt them.");
            if (!parameters.EndsWith(suffix, StringComparison.Ordinal))
                throw new InvalidOperationException("This WGSM host has an unsupported SteamCMD parameter format.");
            return "+@ShutdownOnFailedCommand 1 +@NoPromptForPassword 1 " + parameters.Substring(0, parameters.Length - suffix.Length)
                + string.Concat(items.Select(i => " +workshop_download_item " + appId + " " + i.Id + " validate")) + " +quit";
        }

        internal static async Task<List<Item>> Update(string serverRoot, string configRoot, string steamRoot, string appId,
            Func<string, string, Task<Tuple<int, string>>> runner = null)
        {
            var items = Read(Path.Combine(configRoot, "WorkshopMods.cfg"));
            string state = Path.Combine(configRoot, "workshop-state-" + appId);
            string pending = Path.Combine(state, "pending.txt");
            if (File.Exists(pending))
                throw new InvalidOperationException("An interrupted Workshop deployment needs recovery. See " + pending);
            if (items.Count == 0) return items;
            if (appId != "221100" && appId != "107410") throw new InvalidOperationException("Unsupported Workshop application.");
            NoLinks(serverRoot); NoLinks(configRoot); NoLinks(steamRoot);
            string exe = Path.Combine(steamRoot, "steamcmd.exe");
            if (!File.Exists(exe)) throw new FileNotFoundException("SteamCMD is missing. Install or update the server first.", exe);
            Directory.CreateDirectory(state);
            string cache = Path.Combine(state, "download");
            Directory.CreateDirectory(cache);
            NoLinks(state); NoLinks(cache);
            NoLinks(Path.Combine(steamRoot, "wgsm-workshop.lock"));
            NoLinks(Path.Combine(state, "update.lock"));
            // Shared across both plugins and WGSM instances using this SteamCMD directory.
            using (var cacheLock = new FileStream(Path.Combine(steamRoot, "wgsm-workshop.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            using (var serverLock = new FileStream(Path.Combine(state, "update.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                string parameters = Parameters(cache, appId, items);
                var result = await (runner == null ? Run(exe, parameters) : runner(exe, parameters));
                if (result.Item1 != 0) throw new InvalidOperationException("SteamCMD Workshop update failed. Check account access, Steam Guard and network connectivity.");
                var sources = new Dictionary<string, string>();
                foreach (var item in items)
                {
                    // Exit code zero also occurs on failed Workshop commands. Require an item-specific receipt.
                    if (!Regex.IsMatch(result.Item2, @"Success\.\s+Downloaded item " + item.Id + @"\s+to\s+" , RegexOptions.IgnoreCase))
                        throw new InvalidOperationException("SteamCMD did not confirm Workshop item " + item.Id + ". Server start was cancelled.");
                    var paths = new[] { cache, steamRoot }.Select(root => Path.Combine(root, "steamapps", "workshop", "content", appId, item.Id));
                    string source = paths.FirstOrDefault(path => Directory.Exists(path)
                        && result.Item2.IndexOf(path, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (source == null) throw new DirectoryNotFoundException("Confirmed Workshop content not found for item " + item.Id + ".");
                    sources.Add(item.Id, source);
                }
                Deploy(serverRoot, state, appId, items, sources);
            }
            return items;
        }

        private static async Task<Tuple<int, string>> Run(string exe, string parameters)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo(exe, parameters)
                {
                    WorkingDirectory = Path.GetDirectoryName(exe), UseShellExecute = false, CreateNoWindow = true,
                    RedirectStandardOutput = true, RedirectStandardError = true, RedirectStandardInput = true
                };
                process.Start();
                process.StandardInput.Close();
                var stdout = process.StandardOutput.ReadToEndAsync();
                var stderr = process.StandardError.ReadToEndAsync();
                bool exited = await Task.Run(() => process.WaitForExit(30 * 60 * 1000));
                if (!exited)
                {
                    process.Kill();
                    await Task.Run(() => process.WaitForExit());
                    throw new TimeoutException("Workshop download timed out after 30 minutes. Server start was cancelled.");
                }
                // Keep Steam output in memory. Login details must not be copied into the server log.
                return Tuple.Create(process.ExitCode, await stdout + "\n" + await stderr);
            }
        }

        private static void NoLinks(string path)
        {
            for (string current = Path.GetFullPath(path); !string.IsNullOrEmpty(current); current = Path.GetDirectoryName(current))
                if ((File.Exists(current) || Directory.Exists(current)) && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                    throw new IOException("Workshop paths must not contain links or junctions: " + current);
        }

        private static void CopyTree(string source, string destination)
        {
            NoLinks(source); NoLinks(destination);
            Directory.CreateDirectory(destination);
            foreach (string file in Directory.GetFiles(source))
            {
                NoLinks(file);
                File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), false);
            }
            foreach (string directory in Directory.GetDirectories(source))
                CopyTree(directory, Path.Combine(destination, Path.GetFileName(directory)));
        }

        private static string Hash(string path)
        {
            using (var sha = SHA256.Create())
            using (var stream = File.OpenRead(path))
                return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
        }

        // Back up every replaced folder/key. A failed batch is rolled back before starting the game.
        internal static void Deploy(string root, string state, string appId, List<Item> items, Dictionary<string, string> sources)
        {
            NoLinks(root); NoLinks(state);
            string pending = Path.Combine(state, "pending.txt");
            if (File.Exists(pending)) throw new IOException("Workshop recovery is pending: " + pending);
            Directory.CreateDirectory(state);
            string transaction = Path.Combine(root, ".wgsm-workshop-backups", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N"));
            string stage = Path.Combine(transaction, "prepared"), backup = Path.Combine(transaction, "previous");
            NoLinks(transaction);
            Directory.CreateDirectory(stage); Directory.CreateDirectory(backup);
            var changes = new List<Tuple<string, string>>();
            string manifest = Path.Combine(state, "keys.txt");
            NoLinks(manifest); NoLinks(pending);
            if (Directory.Exists(manifest) || Directory.Exists(pending)) throw new IOException("Workshop state path is a directory.");
            var oldKeys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (File.Exists(manifest))
                foreach (string line in File.ReadAllLines(manifest))
                {
                    string[] parts = line.Split('|');
                    if (parts.Length != 2 || !Regex.IsMatch(parts[0], @"\Awgsm_" + appId + @"_[1-9][0-9]*_[A-Za-z0-9_.-]+\.bikey\z")
                        || !Regex.IsMatch(parts[1], @"\A[0-9a-f]{64}\z") || oldKeys.ContainsKey(parts[0]))
                        throw new IOException("Invalid Workshop key inventory: " + manifest);
                    oldKeys.Add(parts[0], parts[1]);
                }
            var newKeys = new Dictionary<string, string>(oldKeys, StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                string source = sources[item.Id];
                string target = Path.Combine(root, item.Folder);
                NoLinks(target);
                if (File.Exists(target)) throw new IOException("Workshop folder is a file: " + target);
                string prepared = Path.Combine(stage, item.Folder);
                CopyTree(source, prepared);
                if (!Directory.EnumerateFiles(prepared, "*.pbo", SearchOption.AllDirectories).Any())
                    throw new IOException("Workshop item " + item.Id + " has no PBO content. Collections and non-server items are not supported.");
                changes.Add(Tuple.Create(target, prepared));
                string prefix = "wgsm_" + appId + "_" + item.Id + "_";
                var itemKeys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (string key in Directory.GetFiles(prepared, "*.bikey", SearchOption.AllDirectories))
                {
                    string name = prefix + Path.GetFileName(key);
                    if (!Regex.IsMatch(name, @"\Awgsm_" + appId + @"_[1-9][0-9]*_[A-Za-z0-9_.-]+\.bikey\z"))
                        throw new IOException("Unsupported Workshop key filename: " + Path.GetFileName(key));
                    if (itemKeys.ContainsKey(name) && Hash(itemKeys[name]) != Hash(key))
                        throw new IOException("Conflicting Workshop keys: " + name);
                    itemKeys[name] = key;
                }
                foreach (string name in oldKeys.Keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).Union(itemKeys.Keys, StringComparer.OrdinalIgnoreCase))
                {
                    string keyTarget = Path.Combine(root, "keys", name);
                    NoLinks(keyTarget);
                    if (Directory.Exists(keyTarget)) throw new IOException("Workshop key path is a directory: " + name);
                    if (File.Exists(keyTarget) && (!oldKeys.ContainsKey(name) || Hash(keyTarget) != oldKeys[name]))
                        throw new IOException("Workshop key was changed or is not owned by WGSM: " + name);
                    string keyPrepared = null;
                    if (itemKeys.ContainsKey(name))
                    {
                        keyPrepared = Path.Combine(stage, name);
                        File.Copy(itemKeys[name], keyPrepared);
                        newKeys[name] = Hash(keyPrepared);
                    }
                    else newKeys.Remove(name);
                    changes.Add(Tuple.Create(keyTarget, keyPrepared));
                }
            }
            string inventory = Path.Combine(stage, "keys.txt");
            File.WriteAllLines(inventory, newKeys.OrderBy(p => p.Key).Select(p => p.Key + "|" + p.Value), new UTF8Encoding(false));
            changes.Add(Tuple.Create(manifest, inventory));
            File.WriteAllText(pending, "Workshop deployment interrupted. Backup/prepared files: " + transaction + "\n"
                + string.Join("\n", changes.Select(c => c.Item1)) + "\n", new UTF8Encoding(false));
            var moved = new List<Tuple<string, string, bool>>();
            try
            {
                foreach (var change in changes)
                {
                    NoLinks(change.Item1);
                    string saved = Path.Combine(backup, moved.Count.ToString());
                    bool existed = File.Exists(change.Item1) || Directory.Exists(change.Item1);
                    if (existed) Move(change.Item1, saved);
                    moved.Add(Tuple.Create(change.Item1, saved, existed));
                    if (change.Item2 != null)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(change.Item1));
                        Move(change.Item2, change.Item1);
                    }
                }
                File.Delete(pending);
            }
            catch
            {
                bool restored = true;
                foreach (var change in moved.AsEnumerable().Reverse())
                {
                    try
                    {
                        if (Directory.Exists(change.Item1)) Directory.Delete(change.Item1, true);
                        else if (File.Exists(change.Item1)) File.Delete(change.Item1);
                        if (change.Item3) Move(change.Item2, change.Item1);
                    }
                    catch { restored = false; }
                }
                if (restored) File.Delete(pending);
                throw new IOException("Workshop activation failed. " + (restored ? "Previous files were restored." : "Recovery required: " + pending));
            }
        }

        private static void Move(string source, string destination)
        {
            if (Directory.Exists(source)) Directory.Move(source, destination);
            else File.Move(source, destination);
        }
    }
}
