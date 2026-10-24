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
using System.Windows;
using System.Windows.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using WindowsGSM;
using WindowsGSM.Functions;

internal static class ConsoleChecks
{
    private static int passed;
    private static readonly string Root = Path.Combine(AppContext.BaseDirectory, "console-check-" + Guid.NewGuid().ToString("N"));
    private static readonly string Fixture = Path.Combine(AppContext.BaseDirectory, "Fixture", "WindowFixture.exe");
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr window);
    [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr window, uint message, IntPtr first, IntPtr second);
    private static void Check(bool result, string text)
    {
        if (!result) throw new Exception(text);
        passed++;
        Console.WriteLine("PASS " + text);
    }

    private static void Wait(Func<bool> ready, string text)
    {
        var timer = Stopwatch.StartNew();
        while (!ready() && timer.ElapsedMilliseconds < 10000)
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false));
            Dispatcher.PushFrame(frame);
            Thread.Sleep(20);
        }
        Check(ready(), text);
    }

    private static MethodInfo Method(Type type, string name) => type.GetMethod(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
    private static object Snapshot(Type type, string directory) => Method(type, "SnapshotRptFiles").Invoke(null, new object[] { directory });
    private static Task Follow(Type type, Process process, string directory, object snapshot, ConcurrentQueue<string> lines) =>
        Task.Run(() => (Task)Method(type, "FollowRptLog").Invoke(null, new object[] { process, directory, snapshot, new Action<string>(lines.Enqueue) }));

    private sealed class WindowProcess : IDisposable
    {
        public Process Process;
        public IntPtr Window;
        public WindowProcess(string marker)
        {
            Process = Process.Start(new ProcessStartInfo(Fixture, "-fixture=\"" + marker + "\"") {
                UseShellExecute=false, WindowStyle=ProcessWindowStyle.Hidden
            });
            Wait(() => File.Exists(marker) && new FileInfo(marker).Length > 0, "Native window fixture starts");
            Window = (IntPtr)long.Parse(File.ReadAllText(marker));
        }
        public void Close()
        {
            if (!Process.HasExited) PostMessage(Window, 0x10, IntPtr.Zero, IntPtr.Zero);
            Wait(() => Process.HasExited, "Native window fixture exits cleanly");
        }
        public void Dispose()
        {
            if (!Process.HasExited) { PostMessage(Window, 0x10, IntPtr.Zero, IntPtr.Zero); if (!Process.WaitForExit(2000)) Process.Kill(); }
            Process.Dispose();
        }
    }

    private static void Native(Type type, int id)
    {
        using (var child = new WindowProcess(Path.Combine(Root, id + ".window")))
        {
            child.Process.Refresh();
            Check(!IsWindowVisible(child.Window) && child.Process.MainWindowHandle == IntPtr.Zero, "Hidden-window reproduction has no Process.MainWindowHandle");
            var settings = new ServerConfig(id.ToString());
            object dayz = Activator.CreateInstance(type, settings);
            IntPtr found = (IntPtr)Method(type, "FindNativeWindow").Invoke(null, new object[] { child.Process });
            Check(found == child.Window, "DayZ resolves its hidden window by process ownership");
            var metadata = new MainWindow.ServerMetadata { Process=child.Process, ServerConsole=new ServerConsole(id) };
            MainWindow._serverMetadata[id] = metadata;
            Task monitor = Task.Run(() => (Task)Method(type, "MonitorNativeWindow").Invoke(dayz, new object[] { child.Process }));
            Wait(() => metadata.MainWindow == child.Window, "Native monitor registers the real window for Toggle Console");
            Wait(() => File.Exists(ServerPath.GetServersCache(id.ToString(), "windowsIntPtr")), "Native window handle reaches the WGSM cache");
            Check(File.ReadAllText(ServerPath.GetServersCache(id.ToString(), "windowsIntPtr")) == child.Window.ToInt64().ToString(), "Cached handle matches the target process window");
            metadata.ShowConsole = true;
            Wait(() => IsWindowVisible(child.Window), "ShowConsole enables the hidden native window");
            metadata.ShowConsole = false;
            Wait(() => !IsWindowVisible(child.Window), "ShowConsole hides the same native window again");
            metadata.MainWindow = IntPtr.Zero;
            Wait(() => metadata.MainWindow == child.Window, "Native monitor repairs a missing cached handle");
            Task register = (Task)typeof(MainWindow).GetMethod("RegisterNativeWindow", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, new object[] { child.Process, id.ToString(), "DayZ fixture" });
            Wait(() => register.IsCompleted, "Core window registration accepts the plugin's hidden window handle");
            register.GetAwaiter().GetResult();
            Check(!IsWindowVisible(child.Window), "Core registration keeps the saved hidden state");
            metadata.Process = null;
            Wait(() => monitor.IsCompleted, "Native monitor stops when WGSM replaces its process");
            monitor.GetAwaiter().GetResult();
            child.Close();
            Check((IntPtr)Method(type, "FindNativeWindow").Invoke(null, new object[] { child.Process }) == IntPtr.Zero, "An exited process cannot select another server window");
            MainWindow._serverMetadata.Remove(id);
        }
    }

    private static void Rpt(Type type, int id)
    {
        string directory = Path.Combine(Root, id + " profile with spaces");
        Directory.CreateDirectory(directory);
        string old = Path.Combine(directory, "DayZServer_old.RPT");
        File.WriteAllText(old, "history from a previous run\n");
        var snapshot = Snapshot(type, directory);
        Check(Method(type, "FindActiveRptFile").Invoke(null, new[] { (object)directory, snapshot }) == null, "An unchanged old RPT is skipped");
        using (var child = new WindowProcess(Path.Combine(Root, id + ".window")))
        {
            var lines = new ConcurrentQueue<string>();
            Task follow = Follow(type, child.Process, directory, snapshot, lines);
            Wait(() => lines.Any(line => line.Contains("Waiting for DayZ")), "RPT follower starts before a new log exists");
            File.WriteAllText(Path.Combine(directory, "another-game.rpt"), "not DayZ\n");
            Check(Method(type, "FindActiveRptFile").Invoke(null, new[] { (object)directory, snapshot }) == null, "Unrelated RPT files are ignored");
            string active = Path.Combine(directory, "DayZServer_session.RPT");
            File.WriteAllText(active, "first line\r\nTeil", new UTF8Encoding(true));
            Wait(() => lines.Contains("first line"), "New RPT output reaches the console without a UTF-8 BOM");
            Check(!lines.Contains("Teil"), "Incomplete RPT lines wait for their newline");
            File.AppendAllText(active, "stück\r\n", new UTF8Encoding(false));
            Wait(() => lines.Contains("Teilstück"), "Partial writes become one complete console line");
            using (var writer = new FileStream(active, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)) writer.WriteByte(0xE2);
            Thread.Sleep(250);
            using (var writer = new FileStream(active, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)) writer.Write(new byte[] { 0x82, 0xAC, 0x0A });
            Wait(() => lines.Contains("€"), "UTF-8 characters survive a split write");
            Thread.Sleep(300);
            Check(lines.Count(line => line == "first line") == 1, "Polling does not repeat existing RPT lines");
            File.WriteAllText(active, "reset\n");
            Wait(() => lines.Contains("reset"), "RPT truncation resets the read offset");
            string rotated = Path.Combine(directory, "DayZServer_next.rpt");
            File.WriteAllText(rotated, "rotated\n");
            Wait(() => lines.Contains("rotated"), "RPT rotation switches to the new session log");
            using (var locked = new FileStream(rotated, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) Thread.Sleep(350);
            File.AppendAllText(rotated, "unlocked\n");
            Wait(() => lines.Contains("unlocked"), "A temporary file lock does not stop RPT output");
            string reusedDirectory = Path.Combine(Root, id + " separate profile");
            Directory.CreateDirectory(reusedDirectory);
            string reused = Path.Combine(reusedDirectory, "DayZServer_reused.rpt");
            File.WriteAllText(reused, "old reused history\n");
            object reusedSnapshot = Snapshot(type, reusedDirectory);
            var reusedLines = new ConcurrentQueue<string>();
            Task reusedFollow = Follow(type, child.Process, reusedDirectory, reusedSnapshot, reusedLines);
            File.AppendAllText(reused, "new reused output\n");
            Wait(() => reusedLines.Contains("new reused output"), "An appended old RPT starts after the pre-launch snapshot");
            Check(!reusedLines.Contains("old reused history") && !lines.Contains("history from a previous run"), "Old sessions are not replayed into the console");
            Check(!reusedLines.Contains("rotated") && !lines.Contains("new reused output"), "Separate profile directories keep their output separate");
            child.Close();
            File.AppendAllText(rotated, "final shutdown line\n");
            Wait(() => follow.IsCompleted && reusedFollow.IsCompleted, "RPT followers stop after the process exits");
            follow.GetAwaiter().GetResult();
            reusedFollow.GetAwaiter().GetResult();
            Check(lines.Contains("final shutdown line"), "RPT follower drains the final shutdown output");
        }
    }

    private static void Start(Type type, int id, bool embed, bool noLogs)
    {
        string serverId = id.ToString();
        string root = ServerPath.GetServersServerFiles(serverId);
        Directory.CreateDirectory(root);
        Directory.CreateDirectory(Path.Combine(root, "mpmissions", "dayzOffline.chernarusplus"));
        foreach (string file in Directory.GetFiles(Path.GetDirectoryName(Fixture))) File.Copy(file, Path.Combine(root, Path.GetFileName(file)), true);
        File.Copy(Fixture, Path.Combine(root, "DayZServer_x64.exe"), true);
        string marker = Path.Combine(root, "fixture.window");
        var settings = new ServerConfig(serverId) { ServerName="DayZ console fixture", ServerIP="0.0.0.0", ServerPort="2502", ServerQueryPort="2507", ServerMaxPlayer="60", ServerMap="dayzOffline.chernarusplus",
            ServerParam="-profiles=\"profile files\" -fixture=\"" + marker + "\"" + (noLogs ? " -noLogs" : "") };
        object dayz = Activator.CreateInstance(type, settings);
        Check((bool)type.GetField("AllowsEmbedConsole").GetValue(dayz), "DayZ advertises its RPT console support");
        type.GetField("AllowsEmbedConsole").SetValue(dayz, embed);
        var metadata = new MainWindow.ServerMetadata { ServerConsole=new ServerConsole(id) };
        MainWindow._serverMetadata[id] = metadata;
        Process process = ((Task<Process>)type.GetMethod("Start").Invoke(dayz, null)).GetAwaiter().GetResult();
        if (process == null) throw new Exception("Fixture launch failed: " + type.GetField("Error").GetValue(dayz));
        metadata.Process = process;
        IntPtr window = IntPtr.Zero;
        try
        {
            Wait(() => File.Exists(marker) && new FileInfo(marker).Length > 0, "Real DayZ Start launches the isolated fixture");
            window = (IntPtr)long.Parse(File.ReadAllText(marker));
            Check(process.StartInfo.WindowStyle == ProcessWindowStyle.Minimized && !process.StartInfo.RedirectStandardOutput && !process.StartInfo.CreateNoWindow, "DayZ retains a native window without redirecting unsupported streams");
            if (embed && !noLogs)
            {
                Check(process.StartInfo.Arguments.Contains("-doLogs"), "RPT mode adds -doLogs when omitted");
                Wait(() => metadata.ServerConsole.Get().Contains("fixture startup"), "Actual Start mirrors RPT through the WGSM dispatcher and ServerConsole");
            }
            else if (noLogs) Wait(() => metadata.ServerConsole.Get().Contains("Remove -noLogs"), "Disabled RPT logging gives a useful console message");
            else Check(!process.StartInfo.Arguments.Contains("-doLogs"), "Native mode keeps the user's logging arguments unchanged");
            var factory = typeof(ServerConfig).Assembly.GetType("WindowsGSM.GameServer.Data.Class");
            Check(!(bool)Method(factory, "SupportsConsoleInput").Invoke(null, new[] { dayz }), "DayZ log display does not claim to accept console commands");
            Check((bool)Method(factory, "SupportsConsoleInput").Invoke(null, new object[] { new object() }), "Other server console input stays enabled by default");
            PostMessage(window, 0x10, IntPtr.Zero, IntPtr.Zero);
            Wait(() => process.HasExited, "Start fixture stops cleanly");
            if (embed && !noLogs) Wait(() => metadata.ServerConsole.Get().Contains("fixture shutdown"), "Actual Start mirrors the final RPT line");
            else Check(!metadata.ServerConsole.Get().Contains("fixture startup"), "Disabled RPT mirroring does not copy log content");
        }
        finally
        {
            if (!process.HasExited) { if (window != IntPtr.Zero) PostMessage(window, 0x10, IntPtr.Zero, IntPtr.Zero); if (!process.WaitForExit(2000)) process.Kill(); }
            process.Dispose();
            MainWindow._serverMetadata.Remove(id);
        }
    }

    private static Type CompilePlugin(string path)
    {
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator)
            .Concat(new[] { typeof(ServerConfig).Assembly.Location }).Distinct(StringComparer.OrdinalIgnoreCase).Select(file => MetadataReference.CreateFromFile(file));
        var compilation = CSharpCompilation.Create("DayZRptChecks", new[] { CSharpSyntaxTree.ParseText(File.ReadAllText(path), new CSharpParseOptions(LanguageVersion.CSharp6)) }, references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        using (var bytes = new MemoryStream())
        {
            var result = compilation.Emit(bytes);
            if (!result.Success) throw new Exception(string.Join(Environment.NewLine, result.Diagnostics));
            Check(result.Success, "Standalone RPT implementation compiles as C# 6");
            return Assembly.Load(bytes.ToArray()).GetType("WindowsGSM.Plugins.DayZ", true);
        }
    }

    [STAThread]
    private static int Main(string[] args)
    {
        var app = new Application { ShutdownMode=ShutdownMode.OnExplicitShutdown };
        Directory.CreateDirectory(Root);
        int firstId = 1000000 + Math.Abs(Guid.NewGuid().GetHashCode() % 1000000);
        try
        {
            Type[] types = { typeof(ServerConfig).Assembly.GetType("WindowsGSM.GameServer.DAYZ", true), CompilePlugin(args[0]) };
            for (int i = 0; i < types.Length; i++)
            {
                int id = firstId + i * 10;
                Native(types[i], id);
                Rpt(types[i], id + 1);
                Start(types[i], id + 2, true, false);
                Start(types[i], id + 3, false, false);
                Start(types[i], id + 4, true, true);
            }
            Console.WriteLine(passed + " DayZ console checks passed using isolated fixtures.");
            return 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
        finally
        {
            // Only remove directories belonging to this check process.
            foreach (int id in Enumerable.Range(firstId, 15))
            {
                string path = Path.GetFullPath(ServerPath.GetServers(id.ToString()));
                string safe = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "servers")) + Path.DirectorySeparatorChar;
                if (path.StartsWith(safe, StringComparison.OrdinalIgnoreCase) && Directory.Exists(path)) Directory.Delete(path, true);
            }
            if (Directory.Exists(Root)) Directory.Delete(Root, true);
            app.Shutdown();
        }
    }
}
