# Minecraft NeoForge

Pick **Minecraft: NeoForge** under **Install** or **Import**. It installs the NeoForge loader without any mods, so you can put together your own mod list in `mods/`. For the All the Mods 10 pack, pick **Minecraft: All the Mods 10 (ATM10)** instead.

## Choosing the version

`serverfiles/neoforge-version.txt` decides which NeoForge version WindowsGSM installs and updates to:

- `21.1` (the default) follows the newest stable NeoForge for Minecraft 1.21.1.
- Another family such as `21.4` follows the newest stable build of that family.
- A full version such as `21.1.265` stays on exactly that build. This also works for beta builds.

Imported servers stay on the family they already use. Updates never switch to another Minecraft version on their own. Note that NeoForge numbers are not Minecraft numbers. The [official version list](https://maven.neoforged.net/releases/net/neoforged/neoforge/) shows what exists. Before you change the family, check that your world and mods work with it.

## Java

You install Java yourself. WindowsGSM picks the right 64-bit version for your NeoForge build:

| NeoForge | Java |
| --- | --- |
| 20.2 to 20.4 | 17 |
| 20.5 to 21.x | 21 |
| 26.x | 25 |

It looks in `JAVA_HOME`, `PATH` and the usual JDK folders. Newer NeoForge families that WindowsGSM doesn't know yet stop with an error.

## Install

WindowsGSM asks you to accept the Minecraft EULA, downloads the installer from the official NeoForged site, checks its SHA-256 checksum and runs it. The installer downloads more files itself, so the server needs internet access. You don't need a CurseForge pack.

## Settings

Set RAM and other Java options in `user_jvm_args.txt`. Extra start parameters in WindowsGSM are passed to the game, not to Java. WindowsGSM keeps ports and player slots in `server.properties` in sync and leaves your MOTD alone. With **Auto Firewall** on, each server gets a TCP rule for the game and a UDP rule for the query port.

## Updates

Stop the server before you update. You need enough free space for a second complete copy of the server. Updates stop with an error if `eula.txt` doesn't contain `eula=true`.

WindowsGSM first builds the new version in a copy of `serverfiles` next to the live server and keeps your worlds, mods, configs, player lists and Java settings. Only when that worked does it swap the folders. The old `serverfiles` then stays in `servers/<server ID>/neoforge-update-backups/`. WindowsGSM doesn't delete these backups, so clean them up from time to time. If preparing the update fails, your server stays untouched and the prepared copy is kept so you can see what went wrong. `wgsm-neoforge-installer.log` inside it shows what the installer said. If an update gets interrupted halfway, `.wgsm-neoforge-update-recovery.txt` next to `serverfiles` lists what to restore and stops the server from starting until you've had a look.

## Importing an existing server

The server folder needs `user_jvm_args.txt` and an installed `libraries/net/neoforged/neoforge/<version>/win_args.txt`. If it contains several NeoForge versions, write the one to use into `.wgsm-neoforge-version.txt`. Import ATM10 servers through **Minecraft: All the Mods 10 (ATM10)**.

## NeoForge documentation

- [Installing a server](https://docs.neoforged.net/user/docs/server/)
- [Java requirements](https://docs.neoforged.net/user/docs/)
- [Moving to Java 25](https://neoforged.net/news/26.1release/)
