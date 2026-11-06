# DayZ setup

Select **DayZ Dedicated Server** under **Install** or **Import**. For installation and updates, WindowsGSM uses the saved Steam account. That account needs access to dedicated server app `223350`.

## Defaults

| WindowsGSM field | Default | Used by DayZ |
| --- | --- | --- |
| Name | Name entered during installation | hostname in the selected config |
| Server IP | WindowsGSM's local IPv4 address | -ip |
| Server Port | 2302 | -port |
| Query Port | 2305 | steamQueryPort in the selected config |
| Map | dayzOffline.chernarusplus | -mission pointing to that folder under mpmissions |
| Max Players | 60 | maxPlayers in the selected config |

New instances use a port increment of 10. Assign non-overlapping ports when running more than one server. UDP forwarding on the router is still needed when hosting behind NAT. Auto Firewall handles Windows application rules.

WindowsGSM uses these extra launch parameters by default.

```text
-config=serverDZ.cfg -profiles=profile -doLogs -adminLog -freezeCheck
```

WindowsGSM adds the current game port, IP and mission path when it starts the server. Add `-netLog` if you need network logging.

## Configuration and mods

WindowsGSM creates the default config if it is missing. Each time the server starts, it updates `hostname`, `maxPlayers` and `steamQueryPort` to match the settings in the app. It makes a `.bak` copy before changing the file and keeps your passwords, gameplay settings, comments and mission classes.

Put quotes around a custom config or profile path that contains spaces.

```text
-config="config\my server.cfg" -profiles="profiles\main" -doLogs -adminLog -freezeCheck
```

Create the custom config before pointing `-config` at it. Give it its own filename since SteamCMD validation can replace `serverDZ.cfg`. The mission you select in **Map** needs to exist under `mpmissions` and takes precedence over the mission in the config. For Livonia, select `dayzOffline.enoch`.

WindowsGSM combines **Manage Addons** and your explicit `-mod` list, keeping their order and removing duplicates. It handles `-serverMod` separately. To download listed mods and their keys before each start, use [Workshop updates](../operations/workshop.md).

If DZSALModServer.exe is present, WindowsGSM launches it and central Auto Firewall handles both the launcher and DayZServer_x64.exe. Removing the launcher restores the native executable. The per-server firewall switch also removes both automatic rules when disabled.

## Console

Enable **Embed Console** and restart the DayZ server to show its RPT output inside WindowsGSM. The plugin follows the active DayZ RPT under the configured -profiles folder. It handles partial writes, UTF-8 text, log changes and the final shutdown output. Old log sessions are skipped.

RPT mode adds `-doLogs` when it is missing. Remove `-noLogs` if you use this mode. The embedded pane is a log view. Use **Toggle Console** to show or hide the native DayZ window.

## Stopping and verification

Stop first tries CTRL+C and waits for DayZ to exit, then tries closing its window. WindowsGSM can still force termination if neither works. For scheduled shutdowns with player warnings, use DayZ's messages.xml mechanism and verify the shutdown in the RPT log before relying on automatic restarts.

The local checks cover built-in registration, old plugin names, embedded icons, configuration preservation, Windows argument parsing, invalid inputs and per-instance paths. The DayZ setup and bundled server integration have also been tested successfully on the server. The tests/DayZConsole fixtures cover RPT output and hidden-window handling for the built-in server and standalone plugin.

## Bohemia documentation

- [DayZ server configuration](https://community.bistudio.com/wiki/DayZ:Server_Configuration)
- [DayZ server messages](https://community.bistudio.com/wiki/DayZ:Server_Messages)
- [DayZ SteamCMD installation](https://community.bistudio.com/wiki/DayZ:Hosting_a_Linux_Server)
