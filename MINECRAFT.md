# Minecraft Java Edition (Vanilla)

Made by PapaGordon. Select **Minecraft: Java Edition Server** in Install or Import. Existing servergame identifiers keep working; the standalone `[Minecraft_Java.cs]` identifier resolves to this built-in module too.

## Installation and Java

Accept the Minecraft EULA during installation. The server JAR can be downloaded before Java is installed. Starting it requires a 64-bit JDK matching the selected Minecraft release. The module reads Mojang's Java requirement and checks `JAVA_HOME`, common JDK folders and `PATH`. It does not install an unrelated fixed Java version automatically.

By default, installation and updates follow Mojang's latest stable release. To pin an exact stable version, put it into `serverfiles/minecraft-version.txt`, for example `1.21.1`. Delete the file or write `latest-release` to follow stable releases again. Releases from Minecraft 1.18 onwards are supported; snapshots are excluded.

## RAM and server settings

Set RAM in `serverfiles/user_jvm_args.txt`. A new file starts with `-Xms2G` and `-Xmx4G`. Existing JVM argument files are kept. Old WindowsGSM start parameters such as `-Xmx1024M -Xms1024M` remain Java options before `-jar` and override these defaults.

WindowsGSM synchronizes game port, query port and player slots, and enables UDP query for its server status. Custom MOTD, world names, gameplay settings, RCON and bind settings stay as configured in `server.properties`. Configuration is created locally without downloading a template.

Embedded and native consoles are supported. Stop sends `stop`, waits up to 30 seconds, then terminates the process only if it has not exited.

## Updates and recovery

Stop the server and make a world backup before updating or changing versions. Mojang's server download is checked against its size, SHA-1, embedded version and Java requirement before activation. The local version is read from the JAR without relying on a first start or `latest.log`.

Worlds, properties, lists, logs and existing JVM options are untouched by a runtime update. Previous runtime files are kept in `servers/<id>/minecraft-update-backups/<UTC timestamp>-<id>`. These are runtime backups, not world backups, and are not removed automatically.

Failed activation restores the replaced runtime files. If that recovery is interrupted, `.wgsm-minecraft-update-recovery.txt` beside `serverfiles` prevents startup and explains which runtime files to restore. Update does not accept a missing EULA.

## Auto Firewall

The central per-server Auto Firewall setting owns the rules: TCP for the game port and UDP for the query port. Vanilla and ATM10 use separate labels, and each server instance has its own rule names. Vanilla does not add a global exception for the shared Java executable.

## Validation

With a .NET 10 SDK, run the filesystem and process fixtures:

```powershell
dotnet run --project tests/Minecraft/MinecraftChecks.csproj
```

The regular `tests/BuiltInServers` suite also checks Vanilla registration, configuration, the actual plugin compiler and central firewall planning. Pass the standalone Vanilla source as the second optional argument, after the existing DayZ source argument. The firewall tests do not add or remove live rules.

Build WindowsGSM with Visual Studio MSBuild as described in [DEVELOPMENT.md](DEVELOPMENT.md); the application's target remains .NET 8. Live multiplayer and production firewall behavior still need a host test.

On 2026-10-29 the built-in implementation was compared with WindowsGSM.Minecraft_Java and matched apart from the namespace, class, metadata and constant server name needed by the built-in catalog. Both passed 27 fixture checks, including the current official Minecraft 26.3 download, size/SHA-1 validation, import and local version detection. This does not verify a live game session.
