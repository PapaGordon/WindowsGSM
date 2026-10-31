// Made by PapaGordon
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;

internal static class SourceConsoleChecks
{
    [DllImport("kernel32.dll")] private static extern IntPtr GetConsoleWindow();
    [DllImport("kernel32.dll")] private static extern bool SetStdHandle(int kind, IntPtr handle);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr window, int state);
    private static readonly StringBuilder report = new StringBuilder();
    private static readonly Type LogType = typeof(Source).Assembly.GetType("WindowsGSM.GameServer.Engine.SourceConsoleLog", true);
    private static readonly string HostDirectory = Path.GetDirectoryName(Environment.ProcessPath);
    private static readonly string ProbeDirectory = Path.Combine(Path.GetDirectoryName(typeof(SourceConsoleChecks).Assembly.Location), "Probe");
    private const BindingFlags Hidden = BindingFlags.NonPublic | BindingFlags.Instance;
    private static int passed;

    private static void Check(bool result, string description)
    {
        if (!result) throw new Exception(description);
        passed++;
        report.AppendLine("PASS " + description);
    }

    private static void Wait(Func<bool> condition)
    {
        var timer = Stopwatch.StartNew();
        while (!condition())
        {
            if (timer.ElapsedMilliseconds > 8000) throw new Exception("Timed out waiting for console fixture.");
            Thread.Sleep(30);
        }
    }

    private static object Log(string directory) => Activator.CreateInstance(LogType, Hidden, null, new object[] { directory }, null);
    private static Task Follow(object log, Process process, ConcurrentQueue<string> output) =>
        (Task)LogType.GetMethod("Follow", Hidden).Invoke(log, new object[] { process, new Action<string>(output.Enqueue) });

    private static void LogChecks(string directory)
    {
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, "console.log");
        File.WriteAllText(path, "previous run\r\n");
        object log = Log(directory);
        var lines = new ConcurrentQueue<string>();
        using (var sleeper = Process.Start(new ProcessStartInfo("cmd.exe", "/d /c ping -n 30 127.0.0.1 >nul") { UseShellExecute = false, CreateNoWindow = true }))
        {
            Task follow = Follow(log, sleeper, lines);
            try
            {
                File.AppendAllText(path, "new run\r\n");
                Wait(() => lines.Contains("new run"));
                Check(!lines.Contains("previous run"), "Old log output is skipped without clearing the file");
                Check(File.ReadAllText(path).StartsWith("previous run"), "Existing disk log stays intact");
                byte[] unicode = Encoding.UTF8.GetBytes("Grüße 🦊\r\n");
                using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    stream.Write(unicode, 0, 3);
                    stream.Flush();
                    Thread.Sleep(400);
                    Check(!lines.Any(line => line.StartsWith("Gr")), "Partial lines wait for their newline");
                    stream.Write(unicode, 3, unicode.Length - 3);
                }
                Wait(() => lines.Contains("Grüße 🦊"));
                Check(true, "UTF-8 characters survive split writes");
                File.WriteAllText(path, "cleared\r\n");
                Wait(() => lines.Contains("cleared"));
                Check(true, "Cleared logs resume from the beginning");
                string grown = new string('x', 200) + "\r\n";
                File.WriteAllText(path, grown);
                Wait(() => lines.Contains(grown.Trim()));
                Check(true, "A cleared log that grows past the previous offset is detected");
                File.Move(path, path + ".old");
                File.WriteAllText(path, "rotated log\r\n");
                Wait(() => lines.Contains("rotated log"));
                Check(true, "Replaced logs are reopened");
                File.AppendAllText(Path.Combine(directory, "console.txt"), "alternate log\r\n");
                Wait(() => lines.Contains("alternate log"));
                Check(true, "The alternate console.txt filename is supported");
                File.AppendAllText(path, "last line");
                sleeper.Kill(true);
                Check(follow.Wait(5000) && lines.Contains("last line"), "Exit drains the final unterminated log line");
            }
            finally
            {
                if (!sleeper.HasExited) sleeper.Kill(true);
                sleeper.WaitForExit(5000);
                follow.Wait(5000);
            }
        }
    }

    private static void StartChecks(string id)
    {
        string root = ServerPath.GetServersServerFiles(id);
        string game = Path.Combine(root, "garrysmod");
        Directory.CreateDirectory(game);
        foreach (string file in Directory.GetFiles(ProbeDirectory))
            File.Copy(file, Path.Combine(root, Path.GetFileName(file)), true);
        string marker = Path.Combine(root, "probe.txt");
        string exe = Path.Combine(root, "ConsoleProbe.exe");
        using (var old = Process.Start(new ProcessStartInfo(exe, "-probe=\"" + marker + "\"") {
            WorkingDirectory = root, UseShellExecute = false, WindowStyle = ProcessWindowStyle.Minimized,
            RedirectStandardOutput = true, RedirectStandardError = true }))
        {
            try
            {
                bool ended = old.WaitForExit(8000);
                string input = File.Exists(marker) ? File.ReadAllText(marker) : "no marker";
                Check(ended && old.ExitCode == 17 && input.StartsWith("INVALID"),
                    "Old redirected-output launch reproduces the invalid Windows console input handle (" + input + ")");
            }
            finally
            {
                if (!old.HasExited) old.Kill(true);
                old.WaitForExit(5000);
            }
        }
        var settings = new ServerConfig(id) { ServerMap = "gm_construct", ServerPort = "27015", ServerIP = "127.0.0.1", ServerMaxPlayer = "24", ServerParam = "-probe=\"" + marker + "\" +gamemode sandbox" };
        Type gmod = typeof(Source).Assembly.GetType("WindowsGSM.GameServer.GMOD", true);
        var server = (Source)Activator.CreateInstance(gmod, settings);
        server.StartPath = "ConsoleProbe.exe";
        for (int attempt = 0; attempt < 2; attempt++)
        {
            File.Delete(marker);
            server.AllowsEmbedConsole = attempt == 0;
            using (var process = server.Start().GetAwaiter().GetResult())
            {
                try
                {
                    Wait(() => File.Exists(marker) && File.ReadAllText(marker).Length > 0);
                    Check(File.ReadAllText(marker) == "VALID" && !process.HasExited, "Native input buffer is valid with Embed Console " + server.AllowsEmbedConsole);
                    Check(!process.StartInfo.RedirectStandardInput && !process.StartInfo.RedirectStandardOutput && !process.StartInfo.RedirectStandardError && !process.StartInfo.CreateNoWindow,
                        "SRCDS keeps its native console streams");
                    Check(process.StartInfo.WorkingDirectory == root && process.StartInfo.Arguments.Contains("+gamemode sandbox") && process.StartInfo.Arguments.Contains("+map gm_construct"),
                        "Server working directory, gamemode and map are preserved");
                    Check(process.StartInfo.Arguments.Contains("-condebug") == server.AllowsEmbedConsole, "Embedded mode enables console logging only when requested");
                    Wait(() => { process.Refresh(); return process.MainWindowHandle != IntPtr.Zero; });
                    ShowWindow(process.MainWindowHandle, 0);
                    ServerConsole.SendMessageToMainWindow(process.MainWindowHandle, "status");
                    Thread.Sleep(300);
                    server.Stop(process).GetAwaiter().GetResult();
                    bool stopped = process.WaitForExit(5000);
                    Check(stopped && process.ExitCode == 0, "Native console command and graceful quit remain usable");
                    if (server.AllowsEmbedConsole)
                        Check(File.ReadAllText(Path.Combine(game, "console.log")).Contains("command: status"), "Console commands reach the native input buffer");
                }
                finally
                {
                    if (!process.HasExited) process.Kill(true);
                    process.WaitForExit(5000);
                }
            }
        }
    }

    [STAThread]
    private static int Main(string[] args)
    {
        string id = "source-check-" + Guid.NewGuid().ToString("N");
        string root = ServerPath.GetServers(id);
        int exit = 0;
        try
        {
            Check(GetConsoleWindow() == IntPtr.Zero, "Regression runner has no console, matching WindowsGSM's GUI process");
            Check(SetStdHandle(-10, new IntPtr(-1)), "GUI fixture has no inherited console input handle");
            StartChecks(id);
            LogChecks(Path.Combine(root, "log-check"));
            report.AppendLine(passed + " Source console checks passed. No game server was started.");
        }
        catch (Exception e) { report.AppendLine("FAIL " + e); exit = 1; }
        finally
        {
            string safe = Path.GetFullPath(Path.Combine(HostDirectory, "servers")) + Path.DirectorySeparatorChar;
            try
            {
                if (Path.GetFullPath(root).StartsWith(safe, StringComparison.OrdinalIgnoreCase) && Directory.Exists(root)) Directory.Delete(root, true);
            }
            catch (Exception e) { report.AppendLine("FAIL cleanup: " + e.Message); exit = 1; }
            File.WriteAllText(args[0], report.ToString());
        }
        return exit;
    }
}
