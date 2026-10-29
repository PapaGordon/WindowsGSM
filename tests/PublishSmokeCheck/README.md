# Published EXE check

Build this small startup hook in Release and copy the published WindowsGSM.exe into an isolated verification folder. Put the standalone DayZ.cs, ARMA3.cs, ATM10.cs, Minecraft_Java.cs and Minecraft_NeoForge.cs plugin folders under plugins in that folder. Minecraft_Java is a local regression fixture; its deleted public repository is not required.

Set DOTNET_STARTUP_HOOKS to the absolute PublishSmokeCheck.dll path for that one child process. The hook checks all eleven MeFriendos built-in entries and compiles all five standalone sources through the application's own loader. It also invokes the published ATM10 mod merge with isolated fixture JARs: an older installed CC:Tweaked version must be replaced while additional mods remain; both versions supplied by the incoming pack must be preserved for NeoForge discovery. It exits before the WPF UI or any game server starts. Clear the variable after the check. Do not include this hook in the release package or configure it for normal WindowsGSM use.

The child process must return exit code 0 and print all nine PASS lines. This checks the packed application's update code, runtime, icon decoding and compiler dependencies. Fixture directories are removed after the ATM10 checks.
