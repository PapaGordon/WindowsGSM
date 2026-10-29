# DayZ setup

For optional Workshop downloads before each start, see [Workshop updates](WORKSHOP.md). Steam subscriptions alone do not update the server's mod folders.

DayZ remains the existing built-in **DayZ Dedicated Server** entry. A Steam account with access to app 223350 is needed for installation and updates. WindowsGSM uses its saved Steam account settings, so an existing setup may not ask for credentials again.

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

The default additional parameters are:

```text
-config=serverDZ.cfg -profiles=profile -doLogs -adminLog -freezeCheck
```

WindowsGSM inserts the current game port, IP and mission path. Network logging is optional; add -netLog when needed. No CPU count, mods or admin password are hardcoded.

## Configuration and mods

The config is created locally if the default file is missing. Each start synchronizes hostname, maxPlayers and steamQueryPort with the WGSM fields. Passwords, gameplay settings, comments and mission classes stay in the file. Changed files receive a .bak backup; unchanged files are left alone.

Custom paths work, including paths with spaces:

```text
-config="config\my server.cfg" -profiles="profiles\main" -doLogs -adminLog -freezeCheck
```

A custom config must already exist. Give custom configs their own filename: SteamCMD validation can replace the shipped serverDZ.cfg. A mission folder must exist under mpmissions. For example, set Map to dayzOffline.enoch to select the installed Livonia mission. The launch mission takes precedence over the existing template value in the config.

The built-in Manage Addons selection and an explicit -mod list are combined into one argument, keeping their order and removing duplicates. -serverMod stays separate. Install the mod files and their signing keys yourself; there is no automatic Workshop downloader.

If DZSALModServer.exe is present, WindowsGSM launches it and central Auto Firewall handles both the launcher and DayZServer_x64.exe. Removing the launcher restores the native executable. The per-server firewall switch also removes both automatic rules when disabled.

## Console

Enable **Embed Console** and restart the DayZ server to show its RPT output inside WindowsGSM. The plugin follows the active DayZ RPT under the configured -profiles folder. It handles partial writes, UTF-8 text, log changes and the final shutdown output. Old log sessions are skipped.

RPT mode adds -doLogs when missing. Remove -noLogs if present. This view displays logs only; command entry is disabled. Use **Toggle Console** to show or hide the native DayZ window.

## Stopping and verification

Stop first tries CTRL+C and waits for DayZ to exit, then tries closing its window. WindowsGSM can still force termination if neither works. For scheduled shutdowns with player warnings, use DayZ's messages.xml mechanism and verify the shutdown in the RPT log before relying on automatic restarts.

The local checks cover built-in registration, old plugin names, embedded icons, configuration preservation, Windows argument parsing, invalid inputs and per-instance paths. The DayZ setup and bundled server integration have also been tested successfully on the server. The tests/DayZConsole fixtures cover RPT output and hidden-window handling for the built-in server and standalone plugin.

References: [DayZ server configuration](https://community.bistudio.com/wiki/DayZ:Server_Configuration), [DayZ server messages](https://community.bistudio.com/wiki/DayZ:Server_Messages), [DayZ SteamCMD installation](https://community.bistudio.com/wiki/DayZ:Hosting_a_Linux_Server).
