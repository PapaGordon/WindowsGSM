# Source console checks

Build WindowsGSM in Release first, then run on Windows:

```powershell
dotnet build tests/SourceConsole/SourceConsoleChecks.csproj -c Release
$report = Join-Path $env:TEMP 'wgsm-source-console.txt'
$process = Start-Process tests/SourceConsole/bin/Release/net8.0-windows8.0/SourceConsoleChecks.exe -ArgumentList ('"' + $report + '"') -WindowStyle Hidden -PassThru -Wait
Get-Content $report
if ($process.ExitCode -ne 0) { throw 'Source console regression failed' }
```

The GUI runner sets its stdin handle to invalid, matching a GUI host without an inherited console input buffer. A small console probe reproduces the old failure, then checks the actual built-in Garry's Mod start and stop methods. The checks also cover log append, split UTF-8 writes, truncation, replacement and exit draining. Fixtures use isolated server directories and never start SRCDS, SteamCMD or firewall changes. This does not replace a live Garry's Mod server test.
