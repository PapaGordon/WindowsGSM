# Farming Simulator 22 and 25

Pick **Farming Simulator 22 Dedicated Server** or **Farming Simulator 25 Dedicated Server** under **Install** or **Import**. There's no separate server download, the server comes with the normal game. So the Steam account saved in WindowsGSM has to own the game. If someone wants to play with that account while the server runs, use a separate account and license for the server. [Steam account and password](../operations/steam-credentials.md)

| | FS22 | FS25 |
| --- | --- | --- |
| Steam app | `1248130` | `2300320` |
| Game port | `10823` | `10823` to `10825` (UDP) |
| Port step between servers in WindowsGSM | 1 | 3 |

## Install

1. Pick the game and click **Set Account**. Sign in with the account that owns the game. Steam Guard may ask for a code.
2. Click **Install**.
3. Start the server in WindowsGSM.
4. Open the web panel address that the GIANTS server manager shows and set up your game there.

## Settings

Everything that matters for the game, like name, password, port, slots, map and mods, is set in the GIANTS web panel. WindowsGSM doesn't pass ports, slots or a map to the server. Its **Port** and **Max Players** fields are only for display. The web panel's own address and port are in `dedicatedServer.xml`. WindowsGSM never changes your GIANTS settings, savegames or mods.

Before every start, WindowsGSM checks that `dedicatedServer.exe`, the game in `x64\` and `dedicatedServer.xml` exist, and creates `steam_appid.txt` so Steam doesn't ask for confirmation when the server starts the game.

Keep the web panel private. Reach it from your local network or a VPN, not from the internet.

## Updating

1. Stop the server and back up savegame, mods and settings.
2. Click **Update** and enter a Steam Guard code if asked.
3. Start the server and check the web panel and log before players join.

## Console and stopping

With **Embed Console** on, the server manager's output appears in WindowsGSM and its window stays hidden.

**Stop** sends CTRL+C and waits 30 seconds, then closes the server manager's window and waits 10 seconds. Only if the server still runs is the process ended.

## Firewall

With **Auto Firewall** on, WindowsGSM allows both `dedicatedServer.exe` and the game itself (`x64\FarmingSimulator2022Game.exe` or `x64\FarmingSimulator2025Game.exe`) in the Windows firewall. Router port forwarding is up to you.

## More

Both servers are available as standalone plugins for other WindowsGSM builds: [FS22](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator22) and [FS25](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator25).
