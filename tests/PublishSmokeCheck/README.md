# Published EXE check

Build this small startup hook in Release and copy the published WindowsGSM.exe into an isolated verification folder. Put the standalone DayZ.cs plugin folder under plugins in that folder.

Set DOTNET_STARTUP_HOOKS to the absolute PublishSmokeCheck.dll path for that one child process. The hook checks all nine built-in entries and compiles the DayZ plugin through the application's own loader. It exits before the WPF UI or any game server starts. Clear the variable after the check. Do not include this hook in the release package or configure it for normal WindowsGSM use.

The child process must return exit code 0 and print both PASS lines. This checks the packed application's runtime and compiler dependencies, not a live DayZ server.
