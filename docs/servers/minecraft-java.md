# Minecraft Java Edition (Vanilla)

Pick **Minecraft: Java Edition Server** under **Install** or **Import**. Servers created with the old `[Minecraft_Java.cs]` plugin open with the same built-in version.

## Install

1. Accept the Minecraft EULA during installation.
2. Install the 64-bit Java version that your Minecraft release needs. WindowsGSM can download the server before Java is installed, but it can't start without it. It finds Java through `JAVA_HOME`, the usual JDK folders and `PATH`.
3. Start the server.

## Choosing the Minecraft version

By default, WindowsGSM installs and updates to Mojang's newest stable release. To stay on one version, write it into `serverfiles/minecraft-version.txt`, for example `1.21.1`. To follow new releases again, delete the file or write `latest-release` into it. Every stable release from 1.18 on works.

## Memory and settings

Set the RAM in `serverfiles/user_jvm_args.txt`. A new server starts with `-Xms2G` and `-Xmx4G`. An existing file is left alone. Old WindowsGSM start parameters such as `-Xmx1024M -Xms1024M` still work and take priority over that file.

WindowsGSM keeps the game port, query port and player slots in `server.properties` in sync with its own settings, and turns on the query so it can show the server status. Everything else in `server.properties`, like MOTD, world name, gameplay, RCON or bind address, stays as you set it.

Both the embedded console and the server's own window work. **Stop** sends `stop` and gives the server 30 seconds to save and close before ending the process.

## Updates

Stop the server and back up your world before you update or switch versions.

WindowsGSM checks every download from Mojang (size, SHA-1, version and required Java) before it uses it. An update only replaces the server files. Worlds, `server.properties`, player lists, logs and your Java settings stay.

The replaced files are kept in `servers/<server ID>/minecraft-update-backups/`. WindowsGSM doesn't delete these backups, so clean them up from time to time. They don't include your world, so keep your own world backups.

If something fails during an update, WindowsGSM puts the old files back. If that is interrupted too, the file `.wgsm-minecraft-update-recovery.txt` next to `serverfiles` stops the server from starting and lists which files to restore. Updates stop with an error if `eula.txt` doesn't contain `eula=true`.

## Firewall

With **Auto Firewall** on, each server gets a TCP rule for the game port and a UDP rule for the query port. There's no general exception for Java itself, which other programs on the machine would share.
