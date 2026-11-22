# SCP: Secret Laboratory

Pick **SCP: Secret Laboratory Dedicated Server** under **Install** or **Import**. WindowsGSM downloads the server (Steam app `996560`) without a Steam account and runs it through Northwood's LocalAdmin. SCP:SL also needs some Windows runtimes, such as .NET Framework 4.8 and the Visual C++ Redistributable. See the [SCP:SL technical wiki](https://techwiki.scpslgame.com/) for the current list.

## First start

1. Click **Install**. Keep port `7777` or choose another free UDP port.
2. Start the server. LocalAdmin asks you to accept Northwood's EULA. Read it and accept it in the console yourself. WindowsGSM never does that for you.
3. Set up the game in `%APPDATA%\SCP Secret Laboratory\config\<port>\config_gameplay.txt`, in the profile of the Windows user that runs WindowsGSM.

## Settings

| | Default |
| --- | --- |
| Game port | `7777` (UDP). WindowsGSM passes it to LocalAdmin as the first argument. |
| Start parameters | `--useDefault`, which skips LocalAdmin's setup wizard. It does not accept the EULA. |

Don't put a port into the start parameters. The server won't start then and tells you why. Server name, player limit and everything else come from `config_gameplay.txt`, not from WindowsGSM:

```text
server_name: MeFriendos | EU | DE/EN | 16+
server_ip: auto
max_players: 20
contact_email: YOUR_CONTACT_EMAIL
```

If you've read and accepted the EULA and want the server to start without asking, add LocalAdmin's `--acceptEULA` to the start parameters yourself.

## Console and stopping

With **Embed Console** on, WindowsGSM adds `--printStd --noSetCursor --disableTrueColor`, unless you already have them, so LocalAdmin also shows the game's output. Commands you type go to LocalAdmin.

**Stop** sends LocalAdmin's `exit` command and waits 20 seconds, then tries again through its window and waits 20 more seconds. Only then is the process ended.

## Firewall

SCP:SL only needs the game port over UDP. With **Auto Firewall** on, WindowsGSM allows both `LocalAdmin.exe` and `SCPSL.exe` in the Windows firewall. Router port forwarding is up to you.

## Plugins and verification

Current server builds come with Northwood's LabAPI for server plugins, usually in `%APPDATA%\SCP Secret Laboratory\LabAPI\`. WindowsGSM doesn't install or change it. Server verification by Northwood is something you apply for yourself.

## More

The same server is available as a [standalone plugin](https://github.com/PapaGordon/WindowsGSM.SCPSL) for other WindowsGSM builds.
