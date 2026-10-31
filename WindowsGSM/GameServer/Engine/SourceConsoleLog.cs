// Made by PapaGordon
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsGSM.GameServer.Engine
{
    // Keep SRCDS on a real console and follow only this start's log output.
    internal sealed class SourceConsoleLog
    {
        private readonly LogFile[] files;

        internal SourceConsoleLog(string directory)
        {
            files = new[] { "console.log", "console.txt" }.Select(name => new LogFile(Path.Combine(directory, name))).ToArray();
            foreach (var file in files) file.Snapshot();
        }

        internal async Task Follow(Process process, Action<string> output)
        {
            bool ended = false;
            bool warned = false;
            DateTime warnAt = DateTime.UtcNow.AddSeconds(15);
            try
            {
                while (true)
                {
                    foreach (var file in files) file.Read(output);
                    if (!warned && DateTime.UtcNow >= warnAt && files.All(file => !file.ReceivedOutput))
                    {
                        output("[WindowsGSM] Waiting for Source console log output. Check -condebug and any custom con_logfile setting.");
                        warned = true;
                    }
                    // One final pass after exit picks up the last flushed lines.
                    if (ended) break;
                    try { ended = process.HasExited; }
                    catch (InvalidOperationException) { ended = true; }
                    await Task.Delay(250);
                }
                foreach (var file in files) file.Flush(output);
            }
            catch (InvalidOperationException)
            {
                // Closing WGSM must not affect the server process.
            }
        }

        private sealed class LogFile
        {
            private readonly string path;
            private readonly Decoder decoder = Encoding.UTF8.GetDecoder();
            private readonly StringBuilder pending = new StringBuilder();
            private readonly byte[] bytes = new byte[8192];
            private readonly char[] chars = new char[Encoding.UTF8.GetMaxCharCount(8192)];
            private long offset;
            private DateTime created;
            private byte[] boundary = Array.Empty<byte>();
            internal bool ReceivedOutput;

            internal LogFile(string path) { this.path = path; }

            internal void Snapshot()
            {
                try
                {
                    using (var stream = Open())
                    {
                        created = File.GetCreationTimeUtc(path);
                        offset = stream.Length;
                        boundary = ReadBoundary(stream);
                    }
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }

            private FileStream Open() => new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

            private byte[] ReadBoundary(FileStream stream)
            {
                var result = new byte[(int)Math.Min(64, offset)];
                stream.Position = offset - result.Length;
                stream.ReadExactly(result);
                return result;
            }

            internal void Read(Action<string> output)
            {
                try
                {
                    using (var stream = Open())
                    {
                        DateTime newCreated = File.GetCreationTimeUtc(path);
                        // Also detect a cleared log that has already grown past our old offset.
                        if (newCreated != created || stream.Length < offset || !ReadBoundary(stream).SequenceEqual(boundary))
                        {
                            offset = 0;
                            created = newCreated;
                            decoder.Reset();
                            pending.Clear();
                        }
                        stream.Position = offset;
                        int read;
                        int remaining = 1024 * 1024;
                        while (remaining > 0 && (read = stream.Read(bytes, 0, Math.Min(bytes.Length, remaining))) > 0)
                        {
                            ReceivedOutput = true;
                            int count = decoder.GetChars(bytes, 0, read, chars, 0, false);
                            for (int i = 0; i < count; i++)
                            {
                                if (chars[i] == '\n') Flush(output);
                                else if (chars[i] != '\uFEFF') pending.Append(chars[i]);
                                // A broken addon must not create an unbounded line in memory.
                                if (pending.Length >= 16384) Flush(output);
                            }
                            offset = stream.Position;
                            remaining -= read;
                        }
                        boundary = ReadBoundary(stream);
                    }
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }

            internal void Flush(Action<string> output)
            {
                if (pending.Length == 0) return;
                output(pending.ToString().TrimEnd('\r'));
                pending.Clear();
            }
        }
    }
}
