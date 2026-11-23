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

        // Free space that must always be left after the download and the swap.
        // On a full disk SteamCMD loses its saved login (config.vdf) and asks for Steam Guard again.
        internal static long Reserve = 5L * 1024 * 1024 * 1024;
        internal static Func<string, long> FreeSpace = path =>
        {
            try { return new DriveInfo(Path.GetPathRoot(Path.GetFullPath(path))).AvailableFreeSpace; }
            catch (Exception) { return long.MaxValue; }
        };
        internal static TimeSpan RetryDelay = TimeSpan.FromSeconds(15);
        internal static TimeSpan LockWait = TimeSpan.FromMinutes(10);

        // notice gets a message when the server starts with the installed mods because the Workshop update was skipped.
        internal static async Task<List<Item>> Update(string serverRoot, string configRoot, string steamRoot, string appId,
            Func<string, string, Task<Tuple<int, string>>> runner = null, Action<string> notice = null)
        {
            var items = Read(Path.Combine(configRoot, "WorkshopMods.cfg"));
            string state = Path.Combine(configRoot, "workshop-state-" + appId);
            string pending = Path.Combine(state, "pending.txt");
            if (File.Exists(pending))
                throw new InvalidOperationException("An interrupted Workshop deployment needs recovery. See " + pending);
            if (items.Count == 0) return items;
            if (appId != "221100" && appId != "107410") throw new InvalidOperationException("Unsupported Workshop application.");
            NoLinks(serverRoot); NoLinks(configRoot); NoLinks(steamRoot);
            try
            {
                await Download(serverRoot, state, steamRoot, appId, items, runner);
            }
            // Without a half-finished swap, the server starts with the installed mods instead of not at all.
            catch (Exception e) when (!File.Exists(pending) && Installed(serverRoot, items))
            {
                notice?.Invoke("Workshop update skipped, the server uses the installed mods. " + e.Message);
            }
            return items;
        }

        private static async Task Download(string serverRoot, string state, string steamRoot, string appId, List<Item> items,
            Func<string, string, Task<Tuple<int, string>>> runner)
        {
            string exe = Path.Combine(steamRoot, "steamcmd.exe");
            if (!File.Exists(exe)) throw new FileNotFoundException("SteamCMD is missing. Install or update the server first.", exe);
            Directory.CreateDirectory(state);
            string cache = Path.Combine(state, "download");
            Directory.CreateDirectory(cache);
            NoLinks(state); NoLinks(cache);
            NoLinks(Path.Combine(steamRoot, "wgsm-workshop.lock"));
            NoLinks(Path.Combine(state, "update.lock"));
            // Shared across both plugins and WGSM instances using this SteamCMD directory.
            using (var cacheLock = await Lock(Path.Combine(steamRoot, "wgsm-workshop.lock")))
            using (var serverLock = await Lock(Path.Combine(state, "update.lock")))
            {
                foreach (string drive in new[] { cache, serverRoot, steamRoot })
                    if (FreeSpace(drive) < Reserve)
                        throw new IOException("Less than " + Gigabytes(Reserve) + " free on the drive of " + drive + ". Free up disk space.");
                Tuple<int, string> result = null;
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    string parameters = await Task.Run(() => Parameters(cache, appId, items));
                    result = await (runner == null ? Run(exe, parameters) : runner(exe, parameters));
                    // Retry once if SteamCMD never got as far as the login (for example, another instance was still running).
                    if (result.Item1 == 0 || result.Item2.IndexOf("Logging in", StringComparison.OrdinalIgnoreCase) >= 0) break;
                    if (attempt == 0) await Task.Delay(RetryDelay);
                }
                if (result.Item1 != 0) throw new InvalidOperationException("SteamCMD Workshop update failed. Check account access, Steam Guard and network connectivity.");
                var sources = new Dictionary<string, string>();
                foreach (var item in items)
                {
                    // Exit code zero also occurs on failed Workshop commands. Require an item-specific receipt.
                    if (!Regex.IsMatch(result.Item2, @"Success\.\s+Downloaded item " + item.Id + @"\s+to\s+" , RegexOptions.IgnoreCase))
                        throw new InvalidOperationException("SteamCMD did not confirm Workshop item " + item.Id + ".");
                    var paths = new[] { cache, steamRoot }.Select(root => Path.Combine(root, "steamapps", "workshop", "content", appId, item.Id));
                    string source = paths.FirstOrDefault(path => Directory.Exists(path)
                        && result.Item2.IndexOf(path, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (source == null) throw new DirectoryNotFoundException("Confirmed Workshop content not found for item " + item.Id + ".");
                    sources.Add(item.Id, source);
                }
                Deploy(serverRoot, state, appId, items, sources);
            }
        }

        // Waits until another Workshop job using the same SteamCMD has finished.
        private static async Task<FileStream> Lock(string path)
        {
            var deadline = DateTime.UtcNow + LockWait;
            while (true)
            {
                try { return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None); }
                catch (IOException) when (DateTime.UtcNow < deadline) { await Task.Delay(5000); }
            }
        }

        private static bool Installed(string root, List<Item> items)
        {
            try
            {
                return items.All(item =>
                {
                    string folder = Path.Combine(root, item.Folder);
                    return Directory.Exists(folder) && (File.GetAttributes(folder) & FileAttributes.ReparsePoint) == 0
                        && Directory.EnumerateFiles(folder, "*.pbo", SearchOption.AllDirectories).Any();
                });
            }
            catch (Exception) { return false; }
        }

        private static string Gigabytes(long bytes)
        {
            return (bytes / 1024.0 / 1024 / 1024).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + " GB";
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

        // Only touches mods that changed. A failed swap is rolled back before the server starts.
        // Only the latest swap is kept as a backup; older ones are deleted afterwards.
        internal static void Deploy(string root, string state, string appId, List<Item> items, Dictionary<string, string> sources)
        {
            NoLinks(root); NoLinks(state);
            string pending = Path.Combine(state, "pending.txt");
            if (File.Exists(pending)) throw new IOException("Workshop recovery is pending: " + pending);
            Directory.CreateDirectory(state);
            string manifest = Path.Combine(state, "keys.txt");
            NoLinks(manifest); NoLinks(pending);
            if (Directory.Exists(manifest) || Directory.Exists(pending)) throw new IOException("Workshop state path is a directory.");
            var oldKeys = ReadKeys(manifest, appId);
            var changed = items.Where(item => !Unchanged(root, appId, item, sources[item.Id], oldKeys)).ToList();
            if (changed.Count == 0) return;
            long needed = changed.Sum(item => Size(sources[item.Id])) + Reserve;
            long free = FreeSpace(root);
            if (free < needed)
                throw new IOException("Not enough disk space for the Workshop update: " + Gigabytes(needed) + " needed, " + Gigabytes(free) + " free.");
            string backups = Path.Combine(root, ".wgsm-workshop-backups");
            string transaction = Path.Combine(backups, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N"));
            NoLinks(transaction);
            try
            {
                Activate(root, state, appId, changed, sources, oldKeys, transaction);
            }
            catch
            {
                // Clean up prepared copies unless a restore is still pending.
                if (!File.Exists(pending)) Remove(transaction);
                throw;
            }
            Remove(Path.Combine(transaction, "prepared"));
            foreach (string old in Directory.GetDirectories(backups))
                if (!string.Equals(Path.GetFullPath(old), Path.GetFullPath(transaction), StringComparison.OrdinalIgnoreCase))
                    Remove(old);
        }

        private static Dictionary<string, string> ReadKeys(string manifest, string appId)
        {
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
            return oldKeys;
        }

        // Same files (name, size, modified time) and unchanged keys: nothing to do.
        private static bool Unchanged(string root, string appId, Item item, string source, Dictionary<string, string> oldKeys)
        {
            try
            {
                var wanted = Files(source);
                var live = Files(Path.Combine(root, item.Folder));
                if (wanted == null || live == null || wanted.Count != live.Count) return false;
                foreach (var file in wanted)
                {
                    FileInfo other;
                    if (!live.TryGetValue(file.Key, out other) || other.Length != file.Value.Length
                        || other.LastWriteTimeUtc != file.Value.LastWriteTimeUtc) return false;
                }
                string prefix = "wgsm_" + appId + "_" + item.Id + "_";
                var expected = wanted.Values.Where(f => f.Extension.Equals(".bikey", StringComparison.OrdinalIgnoreCase))
                    .Select(f => prefix + f.Name).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var owned = oldKeys.Keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
                if (expected.Count != owned.Count || expected.Except(owned, StringComparer.OrdinalIgnoreCase).Any()) return false;
                return owned.All(name =>
                {
                    string key = Path.Combine(root, "keys", name);
                    return File.Exists(key) && (File.GetAttributes(key) & FileAttributes.ReparsePoint) == 0 && Hash(key) == oldKeys[name];
                });
            }
            catch (IOException) { return false; }
            catch (UnauthorizedAccessException) { return false; }
        }

        // All files in a folder with their relative paths. Returns null if the folder is missing or contains links.
        private static Dictionary<string, FileInfo> Files(string folder)
        {
            var start = new DirectoryInfo(Path.GetFullPath(folder).TrimEnd(Path.DirectorySeparatorChar));
            if (!start.Exists || (start.Attributes & FileAttributes.ReparsePoint) != 0) return null;
            var files = new Dictionary<string, FileInfo>(StringComparer.OrdinalIgnoreCase);
            var open = new Stack<DirectoryInfo>();
            open.Push(start);
            while (open.Count > 0)
                foreach (var entry in open.Pop().GetFileSystemInfos())
                {
                    if ((entry.Attributes & FileAttributes.ReparsePoint) != 0) return null;
                    if (entry is DirectoryInfo directory) open.Push(directory);
                    else files[entry.FullName.Substring(start.FullName.Length)] = (FileInfo)entry;
                }
            return files;
        }

        private static long Size(string folder)
        {
            var files = Files(folder);
            return files == null ? 0 : files.Values.Sum(f => f.Length);
        }

        // Deletes a folder without following links. Errors here are not critical.
        private static void Remove(string path)
        {
            try
            {
                NoLinks(path);
                if (Directory.Exists(path)) RemoveTree(new DirectoryInfo(path));
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        private static void RemoveTree(DirectoryInfo directory)
        {
            foreach (var entry in directory.GetFileSystemInfos())
            {
                if ((entry.Attributes & FileAttributes.ReparsePoint) != 0)
                {
                    if (entry is DirectoryInfo) Directory.Delete(entry.FullName); else File.Delete(entry.FullName);
                }
                else if (entry is DirectoryInfo child) RemoveTree(child);
                else
                {
                    entry.Attributes = FileAttributes.Normal;
                    entry.Delete();
                }
            }
            directory.Attributes = FileAttributes.Directory;
            directory.Delete();
        }

        private static void Activate(string root, string state, string appId, List<Item> items, Dictionary<string, string> sources,
            Dictionary<string, string> oldKeys, string transaction)
        {
            string pending = Path.Combine(state, "pending.txt");
            string manifest = Path.Combine(state, "keys.txt");
            string stage = Path.Combine(transaction, "prepared"), backup = Path.Combine(transaction, "previous");
            Directory.CreateDirectory(stage); Directory.CreateDirectory(backup);
            var changes = new List<Tuple<string, string>>();
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
