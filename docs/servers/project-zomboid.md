# Project Zomboid

Pick **Project Zomboid Dedicated Server** under **Install** or **Import**. WindowsGSM downloads the server (Steam app `380870`) without a Steam account and starts it with the Java that comes with the game, so you don't need to install Java.

## First start

1. Click **Install** and start the server.
2. Project Zomboid asks for a new admin password in the console. Type it into the WindowsGSM console, or into the server window if Embed Console is off.
3. Stop the server and adjust the settings.

## Settings

| | Default |
| --- | --- |
| Game port | `16261` (UDP). WindowsGSM passes it as `-port`. |
| Server name | `servertest`. Add `-servername <name>` to the start parameters for another name. |
| Memory | 16 GB, see below |

**Max Players** from WindowsGSM is not passed to the server. Set the player limit and everything else in the server's INI file.

Project Zomboid keeps its files in `servers\<server ID>\Zomboid` instead of your Windows user folder. The settings are in `Zomboid\Server`:

- `<server name>.ini` for the server: player limit, password, ports, RCON, mods
- `<server name>_SandboxVars.lua` for the world settings

They appear after the first start. Only edit them while the server is stopped. Saves are in `Zomboid\Saves`.

### Memory

By default the server gets 16 GB of RAM. To change that, add `-Xmx` to the start parameters, for example `-Xmx8g -servername MyServer`. WindowsGSM hands `-Xms` and `-Xmx` to Java and everything else to Project Zomboid. If you only set one of them, it's used for both. Restart the server after changing it. If the machine doesn't have enough free memory for the value, the server won't start or runs slowly.

## Console and stopping

**Embed Console** shows the server output, and you can type server commands there.

**Stop** sends CTRL+C, so Project Zomboid saves and closes, and waits 10 seconds. Large worlds can take longer to save. Then use the `quit` command first.

## Firewall

With **Auto Firewall** on, WindowsGSM allows this server's `jre64\bin\java.exe` in the Windows firewall. Open the UDP ports from your INI on your router. If you use RCON, keep its TCP port closed to the internet and only allow it from your local network or a VPN.

## More

The same server is available as a [standalone plugin](https://github.com/PapaGordon/WindowsGSM.ProjectZomboid) for other WindowsGSM builds.
