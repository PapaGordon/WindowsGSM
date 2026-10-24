// Made by PapaGordon
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;

namespace WindowsGSM.GameServer.Bundled
{
    public class FarmingSimulator25 : SteamCMDAgent
    {
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.FarmingSimulator25",
            author = "MeFriendos",
            description = "WindowsGSM plugin for Farming Simulator 25 Dedicated Server",
            version = "0.1.1",
            url = "https://github.com/PapaGordon/WindowsGSM.FarmingSimulator25",
            color = "#80B918"
        };

        public override bool loginAnonymous => false;
        public override string AppId => "2300320";
        public override string StartPath => @"dedicatedServer.exe";

        public FarmingSimulator25(ServerConfig serverData) : base(serverData)
        {
            base.serverData = _serverData = serverData;
        }

        private readonly ServerConfig _serverData;

        public string FullName = "Farming Simulator 25 Dedicated Server";
        public bool AllowsEmbedConsole = true;
        public int PortIncrements = 3;
        public object QueryMethod = null;

        public string Port = "10823";
        public string QueryPort = "10823";
        public string Defaultmap = "";
        public string Maxplayers = "16";
        public string Additional = "";

        private const uint CTRL_C_EVENT = 0;
        private delegate bool ConsoleCtrlDelegate(uint ctrlType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(uint processId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GenerateConsoleCtrlEvent(uint ctrlEvent, uint processGroupId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handler, bool add);

        public async void CreateServerCFG()
        {
            await Task.Run(() => EnsureSteamAppIdFile());
        }

        // Raziel's base Update() assumes SteamCMD returned a process.
        // Handle a missing account or failed SteamCMD start cleanly here.
        public new async Task<Process> Update(bool validate = false, string custom = null)
        {
            var result = await WindowsGSM.Installer.SteamCMD.UpdateEx(
                _serverData.ServerID,
                AppId,
                validate,
                custom: custom,
                loginAnonymous: loginAnonymous
            );

            Process process = result.Item1;
            Error = result.Item2;

            if (process == null)
                return null;

            await Task.Run(() => process.WaitForExit());
            return process;
        }

        public Task<Process> Start()
        {
            string serverFiles = ServerPath.GetServersServerFiles(_serverData.ServerID);
            string serverManager = ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath);
            string gameExecutable = ServerPath.GetServersServerFiles(_serverData.ServerID, @"x64\FarmingSimulator2025Game.exe");
            string serverConfig = ServerPath.GetServersServerFiles(_serverData.ServerID, "dedicatedServer.xml");

            if (!File.Exists(serverManager))
            {
                Error = $"File not found: {serverManager}";
                return Task.FromResult<Process>(null);
            }

            if (!File.Exists(gameExecutable))
            {
                Error = $"File not found: {gameExecutable}";
                return Task.FromResult<Process>(null);
            }

            if (!File.Exists(serverConfig))
            {
                Error = "dedicatedServer.xml is missing. Validate the Farming Simulator 25 installation in WindowsGSM.";
                return Task.FromResult<Process>(null);
            }

            if (!EnsureSteamAppIdFile())
            {
                Error = "steam_appid.txt could not be created. Check the server folder permissions.";
                return Task.FromResult<Process>(null);
            }

            // Raziel writes the current Embed Console state here right before Start().
            bool embedConsole = AllowsEmbedConsole;

            var process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = serverFiles,
                    FileName = serverManager,
                    WindowStyle = embedConsole ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Minimized,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };

            if (embedConsole)
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                var serverConsole = new ServerConsole(_serverData.ServerID);
                process.OutputDataReceived += serverConsole.AddOutput;
                process.ErrorDataReceived += serverConsole.AddOutput;
            }

            try
            {
                process.Start();

                if (embedConsole)
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }

                return Task.FromResult(process);
            }
            catch (FileNotFoundException e)
            {
                Error = $"File not found: {e.Message}";
            }
            catch (UnauthorizedAccessException e)
            {
                Error = $"Access denied: {e.Message}";
            }
            catch (Exception e)
            {
                Error = e.Message;
            }

            process.Dispose();
            return Task.FromResult<Process>(null);
        }

        public async Task Stop(Process process)
        {
            if (process == null)
                return;

            await Task.Run(() =>
            {
                try
                {
                    process.Refresh();
                    if (process.HasExited)
                        return;
                }
                catch
                {
                    return;
                }

                if (TrySendCtrlC(process, 30000))
                    return;

                try
                {
                    process.Refresh();
                    if (!process.HasExited && process.MainWindowHandle != IntPtr.Zero)
                    {
                        process.CloseMainWindow();
                        if (process.WaitForExit(10000))
                            return;
                    }
                }
                catch
                {
                }

                // Only force it down if the normal shutdown paths failed.
                try
                {
                    process.Refresh();
                    if (!process.HasExited)
                        process.Kill();
                }
                catch
                {
                }
            });
        }

        private bool EnsureSteamAppIdFile()
        {
            try
            {
                string path = ServerPath.GetServersServerFiles(_serverData.ServerID, "steam_appid.txt");
                const string expected = "2300320";

                if (File.Exists(path) && string.Equals(File.ReadAllText(path).Trim(), expected, StringComparison.Ordinal))
                    return true;

                File.WriteAllText(path, expected + Environment.NewLine);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TrySendCtrlC(Process process, int timeoutMilliseconds)
        {
            bool attached = false;

            try
            {
                if (process == null || process.HasExited)
                    return true;

                attached = AttachConsole((uint)process.Id);
                if (!attached)
                    return false;

                SetConsoleCtrlHandler(null, true);

                if (!GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0))
                    return false;

                return process.WaitForExit(timeoutMilliseconds);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (attached)
                {
                    try
                    {
                        SetConsoleCtrlHandler(null, false);
                        FreeConsole();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
