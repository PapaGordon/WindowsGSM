# Arma 3

Pick **Arma 3 Dedicated Server** under **Install** or **Import**. WindowsGSM downloads the server (Steam app `233780`) with your saved Steam account and starts the 64-bit `arma3server_x64.exe`. [Steam account and password](../operations/steam-credentials.md)

## Settings

| WindowsGSM field | Default | Used by Arma |
| --- | --- | --- |
| Server Port | `2302` (UDP) | `-port`. The other ports follow from it, see below. |
| Server Name | | `-name`. In Arma that's the **profile** name, not the name in the server browser. |
| Start parameters | `-profiles=ArmaHosts -config=server.cfg` | Add your mods and other Arma parameters here. |

**Max Players** and **Map** are not passed to Arma. Set the public server name, player limit and missions in `server.cfg`:

```cpp
hostname = "MeFriendos Altis Life";
maxPlayers = 64;
```

`-profiles=ArmaHosts` keeps profiles and logs inside the server folder. Each new Arma server gets ports 100 higher than the previous one (`2302`, `2402`, `2502`, ...).

WindowsGSM never changes `server.cfg`, your profiles or missions. For a public server with mods, check at least `BattlEye = 1;`, `verifySignatures = 2;`, `allowedFilePatching = 0;` and `upnp = 0;`. Some headless client setups need `allowedFilePatching = 1`.

## Mods

Add mod folders to the start parameters, for example `-mod=@CBA_A3;@YourMod -serverMod=@YourServerMod`. To have WindowsGSM download Workshop mods and keep them updated before every start, use a mod list. [Workshop mods](../operations/workshop.md)

## Console

- **Embed Console** shows Arma's log (RPT) from the profile folder in WindowsGSM. Without `-profiles` it looks in `%LOCALAPPDATA%\Arma 3`. It only shows output, Arma doesn't take commands there. With `-noLogs` it stays empty.
- **Toggle Console** shows or hides Arma's own console window. WindowsGSM keeps looking for that window while the server runs, because Arma often opens it late. If it doesn't work, see `servers\<server ID>\cache\arma3-toggle-console.log`.

**Stop** asks Arma's window to close and waits up to 20 seconds before WindowsGSM ends the process.

## Ports and firewall

With the default game port Arma uses UDP `2302` (game and voice), `2303` (Steam query), `2304` (Steam master), `2305` (voice) and `2306` (BattlEye). They move together when you change the game port.

With **Auto Firewall** on, WindowsGSM allows `arma3server_x64.exe` of this server in the Windows firewall. Router port forwarding is up to you.

## More

The same server is available as a [standalone plugin](https://github.com/PapaGordon/WindowsGSM.ARMA3) for other WindowsGSM builds. Its README has more troubleshooting tips.
