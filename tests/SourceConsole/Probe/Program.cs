// Made by PapaGordon
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

internal static class Program
{
    [DllImport("kernel32.dll")] private static extern IntPtr GetStdHandle(int kind);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool GetNumberOfConsoleInputEvents(IntPtr input, out uint count);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern bool ReadConsoleInputW(IntPtr input, out InputRecord record, uint length, out uint read);

    [StructLayout(LayoutKind.Explicit, Size = 20)]
    private struct InputRecord
    {
        [FieldOffset(0)] public ushort EventType;
        [FieldOffset(4)] public int KeyDown;
        [FieldOffset(8)] public ushort RepeatCount;
        [FieldOffset(10)] public ushort VirtualKey;
        [FieldOffset(14)] public char Character;
    }

    private static int Main(string[] args)
    {
        string marker = args.First(arg => arg.StartsWith("-probe=")).Substring(7);
        // This is the same native API check that fails in SRCDS after initialization.
        bool valid = GetNumberOfConsoleInputEvents(GetStdHandle(-10), out _);
        File.WriteAllText(marker + ".tmp", valid ? "VALID" : "INVALID:" + Marshal.GetLastWin32Error());
        File.Move(marker + ".tmp", marker, true);
        if (!valid) return 17;
        string log = Path.Combine(Environment.CurrentDirectory, "garrysmod", "console.log");
        bool logging = args.Contains("-condebug");
        if (logging) File.AppendAllText(log, "probe initialized\r\n");
        var input = GetStdHandle(-10);
        string command = "";
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < TimeSpan.FromSeconds(20))
        {
            if (GetNumberOfConsoleInputEvents(input, out uint events) && events > 0 && ReadConsoleInputW(input, out var record, 1, out _) && record.EventType == 1 && record.KeyDown != 0)
            {
                if (record.Character == '\r' || record.VirtualKey == 13)
                {
                    if (logging) File.AppendAllText(log, "command: " + command + "\r\n");
                    if (command == "quit")
                    {
                        if (logging) File.AppendAllText(log, "probe stopped\r\n");
                        return 0;
                    }
                    command = "";
                }
                else if (record.Character != '\0') command += new string(record.Character, Math.Max(1, (int)record.RepeatCount));
            }
            Thread.Sleep(20);
        }
        return 18;
    }
}
