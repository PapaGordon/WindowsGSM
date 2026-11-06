# DayZ and Arma 3 Workshop updates

Starting with WindowsGSM v1.25.4.4, DayZ and Arma 3 can update your configured Workshop mods before each start, including scheduled restarts. Put the mods you want in a per-server list. **Update on start** still controls updates to the game server itself.

## Setup

1. Stop the server and keep a backup.
2. Set up a Steam account in WindowsGSM that can access the Workshop items. Steam Guard or missing access can stop unattended downloads. [Steam password setup](steam-credentials.md)
3. Create `servers/<server ID>/configs/WorkshopMods.cfg` beside that server's other WGSM configuration files.
4. Add one Workshop item per line. Put dependencies before the mods that use them.

```text
# Replace these example IDs and folder names with your own mods.
123456789|@Dependency|mod
234567891|@Gameplay|mod
345678912|@AdminTools|servermod
```

Use `item ID|@target folder|mod` for a normal mod and `item ID|@target folder|servermod` for a server-only mod. Copy the numeric `id=` from the individual mod's Workshop URL. Use letters, numbers, underscores or hyphens in the folder name after `@`.

WindowsGSM adds the entries to the matching `-mod` or `-serverMod` arguments. Keep using your manual entries or `DayZActivatedMods.cfg` for mods you manage yourself. List each mod through one method.

With no entries, WindowsGSM starts the server as usual. When you remove an entry, WindowsGSM stops updating that folder. You can remove the files and any manually configured launch argument yourself.

## What gets replaced

Each listed target folder is managed as a complete Workshop copy. Its previous contents, including files removed by the mod author, are replaced. Keep persistent mod settings outside these folders, as the mod's own instructions require. **Unlisted mod folders and manual signing keys are kept.**

WindowsGSM checks SteamCMD's exit code and a download receipt for each item before replacing any live mod folder. If a download fails, it cancels the start. If replacement fails, it restores the previous folders and keys.

Downloads have a 30-minute timeout and run without an interactive Steam login. Only one Workshop job can use the same SteamCMD folder at a time. If another job is running, wait for it to finish and retry the start. Run manual SteamCMD jobs separately.

WindowsGSM puts included `.bikey` files in `serverfiles/keys` with the prefix `wgsm_<game app ID>_<item ID>_`. It tracks them by SHA-256 hash and replaces them on subsequent updates. An edited managed key causes a conflict. For keys you installed yourself, check which mods still use them before removing any.

DayZ downloads use Workshop app **221100**, Arma 3 uses **107410**. The dedicated server executable still uses its normal server app ID. No account subscription changes are made, collections are not expanded, and mod-specific installation scripts are not run.

## Backups and recovery

Previous files remain under `serverfiles/.wgsm-workshop-backups/<timestamp-id>/previous`. The `prepared` folder contains staged files that have not been activated. Backups are kept so you can decide when to remove them after checking the server.

If WGSM or the machine stops during activation, the next start refuses to proceed while `configs/workshop-state-<game app ID>/pending.txt` exists. It lists the transaction directory and destination paths in order. Numbered files/folders in `previous` correspond to that list starting at zero. While the server is stopped, restore the available previous entries to their listed destinations and check every destination before removing `pending.txt`. Entries without a previous copy may have been newly installed. Keep the whole transaction until recovery is confirmed. Do not delete the marker just to bypass recovery.

The same updater is available in the standalone plugins for compatible builds. Use MeFriendos WindowsGSM v1.25.4.2 or newer for encrypted credentials. Other builds need their own support for the encrypted field. [DayZ plugin](https://github.com/PapaGordon/WindowsGSM.DayZ) | [Arma 3 plugin](https://github.com/PapaGordon/WindowsGSM.ARMA3)

Checks use isolated download/process fixtures, synthetic encrypted credentials and filesystem rollback cases. Real Steam authentication, Steam Guard, downloads from an account and multiplayer joins still need checking on the target server.
