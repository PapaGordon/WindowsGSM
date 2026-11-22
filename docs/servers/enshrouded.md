# Enshrouded

Pick **Enshrouded Dedicated Server** under **Install** or **Import**. WindowsGSM downloads the server (Steam app `2278520`) without a Steam account.

## Settings

Before every start, WindowsGSM writes four values into `serverfiles\enshrouded_server.json`:

| WindowsGSM field | Default | In the config |
| --- | --- | --- |
| Server Name | | `name`. Must not be empty. |
| Server IP | | `ip`. Must be an IPv4 address, `0.0.0.0` listens on all interfaces. |
| Server Query Port | `15637` (UDP) | `queryPort`. Players connect through this port. |
| Server Max Players | `16` | `slotCount`, between 1 and 16 |

**Server Port** and **Map** have no effect on Enshrouded. Everything else, like game settings, bans or save folders, you change in the JSON file while the server is stopped. The server won't start if the file has an error or one of the four values is invalid.

## Passwords and roles

On first setup, the config gets four roles (Admin, Friend, Guest, Visitor), each with its own random password. Which password a player uses decides their role. Give normal players the Friend password and keep the Admin password to yourself. Look them up in `enshrouded_server.json`.

When one of the four values changes, the previous config is saved as `enshrouded_server.json.bak` first. It contains the passwords too. It's not a world backup, so back up your save folder before game updates.

## Console and stopping

With **Embed Console** on, the server output appears in WindowsGSM and the server's window stays hidden. **Toggle Console** is disabled then.

**Stop** sends CTRL+C and gives the server 30 seconds to save, then tries once more and waits 10 seconds. If the server still runs, the log shows a notice and WindowsGSM ends the process.

## Firewall

With **Auto Firewall** on, WindowsGSM allows `enshrouded_server.exe` of this server in the Windows firewall. Router port forwarding for the query port is up to you.

## More

The same server is available as a [standalone plugin](https://github.com/PapaGordon/WindowsGSM.Enshrouded) for other WindowsGSM builds.
