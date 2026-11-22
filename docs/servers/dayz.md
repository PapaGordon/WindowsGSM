# DayZ

Pick **DayZ Dedicated Server** under **Install** or **Import**. WindowsGSM downloads the server with your saved Steam account, so that account needs access to the DayZ server (Steam app `223350`). [Steam account and password](../operations/steam-credentials.md)

## Settings in WindowsGSM

These fields in WindowsGSM control the server. On every start, WindowsGSM writes them into the server config or passes them as start parameters.

| WindowsGSM field | Default | Ends up in |
| --- | --- | --- |
| Name | The name you enter when installing | `hostname` in the config |
| Server IP | The host's local IPv4 address | `-ip` |
| Server Port | 2302 | `-port` |
| Query Port | 2305 | `steamQueryPort` in the config |
| Map | `dayzOffline.chernarusplus` | `-mission`, pointing to that folder in `mpmissions` |
| Max Players | 60 | `maxPlayers` in the config |

Each new DayZ server gets ports 10 higher than the previous one. If you run several servers, make sure their ports don't overlap. Behind a router you still have to forward the UDP ports yourself. **Auto Firewall** takes care of the Windows firewall.

## Start parameters

New servers start with:

```text
-config=serverDZ.cfg -profiles=profile -doLogs -adminLog -freezeCheck
```

Port, IP and mission are added by WindowsGSM, so leave them out. Add `-netLog` if you need network logs.

To use your own config or profile folder, put the path in quotes if it contains spaces:

```text
-config="config\my server.cfg" -profiles="profiles\main" -doLogs -adminLog -freezeCheck
```

Create the config file before you point `-config` at it. Give it its own name, because a Steam validation can overwrite `serverDZ.cfg`.

## The server config

If the config file doesn't exist yet, WindowsGSM creates one. On every start it only updates `hostname`, `maxPlayers` and `steamQueryPort` and saves the previous version as `.bak` first. Passwords, gameplay settings, comments and mission classes stay exactly as you wrote them.

The mission you select under **Map** has to exist in `mpmissions` and wins over the mission in the config. For Livonia, select `dayzOffline.enoch`.

## Mods

WindowsGSM combines the mods from **Manage Addons** with the ones in your `-mod` parameter, keeps their order and removes duplicates. `-serverMod` is handled separately.

To have WindowsGSM download your mods from the Workshop and keep them updated, use a mod list. [Workshop mods](../operations/workshop.md)

If `DZSALModServer.exe` is in the server folder, WindowsGSM starts the server through it. **Auto Firewall** then allows both the launcher and `DayZServer_x64.exe`. Delete the launcher to go back to the normal server.

## Console

Turn on **Embed Console** and restart the server to see DayZ's log (the RPT file) in WindowsGSM. It follows the newest RPT in your profile folder and skips older sessions. If `-doLogs` is missing, WindowsGSM adds it. With `-noLogs` there's nothing to show, so remove it.

The pane shows the log only, DayZ doesn't take commands there. **Toggle Console** shows or hides DayZ's own window.

## Stopping the server

**Stop** first sends CTRL+C and waits for DayZ to shut down, then tries to close its window. Only if both fail does WindowsGSM end the process. For planned shutdowns with warnings for your players, use DayZ's `messages.xml` and check in the RPT log that the server shuts down cleanly before you rely on automatic restarts.

## Bohemia's documentation

- [Server configuration](https://community.bistudio.com/wiki/DayZ:Server_Configuration)
- [Server messages](https://community.bistudio.com/wiki/DayZ:Server_Messages)
- [Installing the server with SteamCMD](https://community.bistudio.com/wiki/DayZ:Hosting_a_Linux_Server)
