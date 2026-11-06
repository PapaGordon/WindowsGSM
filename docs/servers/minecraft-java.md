# Minecraft Java Edition (Vanilla)

Select **Minecraft: Java Edition Server** under **Install** or **Import**. Existing servers using the `[Minecraft_Java.cs]` plugin name open with the same built-in module.

## Installation and Java

Accept the Minecraft EULA during installation. You can download the server JAR before installing Java. To start the server, install the 64-bit JDK required by that Minecraft release. WindowsGSM reads the Java requirement from Mojang and looks in `JAVA_HOME`, common JDK folders and `PATH`.

Installation and updates follow Mojang's latest stable release by default. To stay on one version, put a release number such as `1.21.1` into `serverfiles/minecraft-version.txt`. Remove the file or write `latest-release` to follow stable releases again. You can select stable releases from Minecraft 1.18 onward.

## RAM and server settings

Set RAM in `serverfiles/user_jvm_args.txt`. A new file starts with `-Xms2G` and `-Xmx4G`. Existing JVM argument files are kept. Old WindowsGSM start parameters such as `-Xmx1024M -Xms1024M` remain Java options before `-jar` and override these defaults.

WindowsGSM synchronizes game port, query port and player slots, and enables UDP query for its server status. Custom MOTD, world names, gameplay settings, RCON and bind settings stay as configured in `server.properties`. Configuration is created locally without downloading a template.

Embedded and native consoles are supported. Stop sends `stop`, waits up to 30 seconds, then terminates the process only if it has not exited.

## Updates and recovery

Stop the server and make a world backup before updating or changing versions. Mojang's server download is checked against its size, SHA-1, embedded version and Java requirement before activation. The local version is read from the JAR without relying on a first start or `latest.log`.

Updates replace the runtime and keep your worlds, properties, lists, logs and JVM options. Look in `servers/<id>/minecraft-update-backups/<UTC timestamp>-<id>` for the previous runtime files. WindowsGSM keeps these backups until you remove them. Keep a separate backup of your world.

Failed activation restores the replaced runtime files. If that recovery is interrupted, `.wgsm-minecraft-update-recovery.txt` beside `serverfiles` prevents startup and explains which runtime files to restore. Update does not accept a missing EULA.

## Auto Firewall

The central per-server Auto Firewall setting owns the rules: TCP for the game port and UDP for the query port. Vanilla and ATM10 use separate labels, and each server instance has its own rule names. Vanilla does not add a global exception for the shared Java executable.

## Validation

Run the filesystem and process checks from the repository root with the .NET 10 SDK.

```powershell
dotnet run --project tests/Minecraft/MinecraftChecks.csproj
```

The regular `tests/BuiltInServers` suite also checks Vanilla registration, configuration, the actual plugin compiler and central firewall planning. Pass the standalone Vanilla source as the second optional argument, after the existing DayZ source argument. The firewall tests do not add or remove live rules.

Build the .NET 8 application with Visual Studio MSBuild. Live multiplayer and firewall behavior need a test on the host. [Build instructions](../development/build-and-release.md)

On 2026-10-29 the built-in implementation was compared with WindowsGSM.Minecraft_Java and matched apart from the namespace, class, metadata and constant server name needed by the built-in catalog. Both passed 27 fixture checks, including the current official Minecraft 26.3 download, size/SHA-1 validation, import and local version detection. This does not verify a live game session.
