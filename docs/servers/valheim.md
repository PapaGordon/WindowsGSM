# Valheim

Pick **Valheim Dedicated Server** under **Install** or **Import**. WindowsGSM downloads the server (Steam app `896660`) without a Steam account. It's set up for the normal Steam network, with or without BepInEx mods, and without Crossplay.

## Settings

| WindowsGSM field | Default | Passed to Valheim as |
| --- | --- | --- |
| Server Name | | `-name` |
| Server Port | `2456` (UDP) | `-port`. Valheim also uses the next port, `2457`. |
| Map | `Dedicated` | `-world`, the name of your world |

New servers start with:

```text
-password "CHANGE_ME" -savedir ".\save-data" -public 1 -saveinterval 1800 -backups 4 -backupshort 7200 -backuplong 43200
```

Replace `CHANGE_ME` with your password before the first start. The server won't start while the password is missing, shorter than 5 characters or still the placeholder. Worlds are saved in `serverfiles\save-data`.

Valheim always allows up to 10 players, **Max Players** doesn't change that. Don't add `-crossplay`, the server refuses to start with it. Each new Valheim server gets ports 2 higher than the previous one.

## Mods with BepInEx

Copy the contents of [BepInExPack Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/) into the server folder, so `BepInEx\`, `doorstop_config.ini` and `winhttp.dll` are next to `valheim_server.exe`. BepInEx then loads on its own. Copy only server-compatible mods from your r2modman or Thunderstore profile into `BepInEx\plugins` and `BepInEx\config`. Check `BepInEx\LogOutput.log` if something doesn't load.

## Console and stopping

With **Embed Console** on, the server output appears in WindowsGSM and Valheim's window stays hidden. **Toggle Console** is disabled then. `-logFile` is ignored while Embed Console is on, because it would send the output to a file instead.

**Stop** sends CTRL+C and gives Valheim 20 seconds to save, then tries once more and waits 10 seconds. Only then is the process ended.

## Firewall

Valheim needs the game port and the one after it over UDP, by default `2456` and `2457`. It has no RCON. With **Auto Firewall** on, WindowsGSM allows `valheim_server.exe` of this server in the Windows firewall. Router port forwarding is up to you.

## More

The [standalone plugin's README](https://github.com/PapaGordon/WindowsGSM.Valheim) has more about BepInEx and troubleshooting. The code is the same.
