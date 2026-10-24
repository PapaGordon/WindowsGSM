# Built-in server checks

Build the WindowsGSM Release configuration first. These checks use that actual assembly and run with .NET 8 on Windows. They do not need Steam credentials or installed games.

```powershell
msbuild tests/BuiltInServers/ServerChecks.csproj /restore /p:Configuration=Release
& tests/BuiltInServers/bin/Release/net8.0-windows8.0/ServerChecks.exe
```

If using a different build folder, pass /p:WindowsGSMDirectory=<absolute path>. To also compile and check the standalone DayZ plugin, pass its source path to the executable:

```powershell
& tests/BuiltInServers/bin/Release/net8.0-windows8.0/ServerChecks.exe '..\WindowsGSM.DayZ\DayZ.cs\DayZ.cs'
```

The tests create uniquely named fixture directories beneath the test executable and remove only those directories. Registration checks use the compiled factory and embedded resources. DayZ checks exercise config synchronization, backups, invalid inputs, Windows command-line parsing, mission paths, merged mods and executable selection. Standalone checks also compile the plugin as C# 6 against the real API.

Firewall checks inspect executable paths, per-server ATM10 port rules and detached COM rule properties without adding or changing Windows rules. They also check missing-rule lookup and failed SteamCMD updates. No DayZ game server is started. SteamCMD login/downloads, actual shutdown and storage saving, real firewall permissions, A2S responses and client connectivity need a test installation.
