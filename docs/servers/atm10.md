# Minecraft: All the Mods 10 (ATM10)

Pick **Minecraft: All the Mods 10 (ATM10)** under **Install** or **Import**. WindowsGSM installs the ATM10 server pack (NeoForge, Minecraft 1.21.1) from the official ZIP, sets up NeoForge and runs the server with Java 21. No Steam account needed.

## Install

1. Install 64-bit Java 21 or newer.
2. Download the server pack `ServerFiles-<version>.zip` from [CurseForge](https://www.curseforge.com/minecraft/modpacks/all-the-mods-10). It's a separate download, not the normal modpack.
3. Put the ZIP into the WindowsGSM folder or the server's `serverfiles` folder.
4. Click **Install** and accept the Minecraft EULA.
5. Set the memory in `user_jvm_args.txt`, for example `-Xms8G` and `-Xmx12G`, and start the server.

WindowsGSM keeps game port, query port and player limit in `server.properties` in sync. The default port is `25565`. Players need the same ATM10 version as the server.

## Updates

1. Stop the server and back up your world.
2. Put the new `ServerFiles-<version>.zip` next to the old one. Leave it unchanged.
3. Click **Update** and read the message WindowsGSM shows afterwards.

The pack version is read from inside the ZIP. The update is prepared in a copy of the server folder and only swapped in when NeoForge installed successfully. The previous folder stays in `servers\<server ID>\atm10-update-backups\`. These backups are never deleted automatically. You need room for a second complete copy of the server.

- **Your own mods** in `mods` stay. WindowsGSM remembers which files came from the pack and only replaces or removes those. If one of your JARs also contains a mod from the pack, the update stops before anything changes.
- **Configs** you never changed get the new pack version. Files you changed stay. If both you and the pack changed a file, yours stays and the update tells you. Compare `config-conflicts.txt` and the `incoming-config` folder in the backup before starting.
- **Always from the pack:** the version that Better Compatibility Checker reports and `config/neovitae/materials.json`, because clients have to match.

The first update from an older WindowsGSM version needs the old original ZIP next to the new one, so WindowsGSM can tell pack mods from yours. Never build that ZIP from your own server folder.

If something goes wrong while swapping the folders, `.wgsm-atm10-update-recovery.txt` next to `serverfiles` lists what to restore and blocks the start until you've checked it.

## Firewall

With **Auto Firewall** on, WindowsGSM creates two rules for this server: TCP for the game port and UDP for the query port. There's no general exception for Java. Router port forwarding is up to you.

## More

The [standalone plugin's README](https://github.com/PapaGordon/WindowsGSM.ATM10) explains mod ownership, config handling and the Neo Vitae change in ATM10 8.3 in more detail. The code is the same.
