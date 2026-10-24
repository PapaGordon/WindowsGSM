# DayZ console checks

Build WindowsGSM in Release first, then run:

```powershell
msbuild tests/DayZConsole/ConsoleChecks.csproj /restore /p:Configuration=Release
& tests/DayZConsole/bin/Release/net8.0-windows8.0/ConsoleChecks.exe '..\WindowsGSM.DayZ\DayZ.cs\DayZ.cs'
```

Use /p:WindowsGSMDirectory=<build folder> when testing a different app build.

These checks compile the standalone plugin as C# 6, then exercise both implementations. The fixtures create their own hidden windows and profile directories. They check missing native handles, visibility changes, cache registration, RPT partial writes, UTF-8, rotation, temporary file locks, separate profiles and final output. The real Start method also runs a small fixture executable to verify the WGSM dispatcher and ServerConsole connection.

The fixtures close their own processes and remove only their own directories. No game installation, Steam login or firewall changes are required.
