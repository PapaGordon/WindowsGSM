# Published EXE check

Build this small startup hook in Release and copy the published WindowsGSM.exe into an isolated verification folder. Put the standalone DayZ.cs, Minecraft_Java.cs and Minecraft_NeoForge.cs plugin folders under plugins in that folder.

Set DOTNET_STARTUP_HOOKS to the absolute PublishSmokeCheck.dll path for that one child process. The hook checks all eleven MeFriendos built-in entries and compiles all three standalone sources through the application's own loader. It exits before the WPF UI or any game server starts. Clear the variable after the check. Do not include this hook in the release package or configure it for normal WindowsGSM use.

The child process must return exit code 0 and print all six PASS lines. Include the standalone ATM10 folder as well. This checks the packed application's runtime, icon decoding and compiler dependencies.
