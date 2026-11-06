# Schedule server tasks with Crontab

Use a CSV schedule to restart a server, run a Windows command or send a command to the game while it is running.

1. Start the server once so WindowsGSM can create `servers/<id>/configs/Crontab/`.
2. Select the server and open **Browse > Server Configs**. Put one or more `.csv` files in the `Crontab` folder.
3. Enable **Restart Crontab** for that server. Restart the server after editing the files so WindowsGSM reloads them.

The restart schedule you enter in the WindowsGSM window is a separate entry. The **Restart Crontab** switch enables it and the CSV schedules.

## File format

Put one entry on each line. Start with a five-field cron expression, then add the action and any arguments, separated by semicolons. Lines starting with `//` are comments. Schedules use the Windows host's local time.

| Action | What goes after it |
| --- | --- |
| `restart` | The line ends after the action. |
| `exec` | The executable and then its arguments, if needed. |
| `ServerConsoleCommand` | The command to send to the server console. |
| `RconCommand` | The command to send over RCON. |

## Examples

Restart once a day at 02:00 and run a local script once a day at 01:05.

```csv
0 2 * * *;restart
5 1 * * *;exec;cmd.exe;/C "C:\Scripts\backup.bat"
```

For ARK, send a five-minute warning, save the world and then restart at 06:10.

```csv
5 6 * * *;ServerConsoleCommand;cheat serverchat Server will restart in 5 minutes
9 6 * * *;ServerConsoleCommand;cheat saveworld
10 6 * * *;restart
```

## RCON setup

Enable RCON in the game server first. Close WindowsGSM, then set the RCON host, port and password in that server's `configs/WindowsGSM.cfg`. Use the address the game actually listens on. If the plugin already generated an RCON password, use that value.

```ini
rconip="127.0.0.1"
rconport="25575"
rconpassword="YOUR_RCON_PASSWORD"
```

After starting WindowsGSM again, this schedule sends a message at five minutes past every hour.

```csv
5 * * * *;RconCommand;say This message runs every hour
```

Some games use a different RCON protocol. If the game's own client works and WindowsGSM logs a timeout, you can run that client with `exec`.

Leave enough time between tasks. Use commands that finish on their own, since WindowsGSM otherwise keeps them running until the server restarts. Programs launched by `exec` run as the WindowsGSM user.

Use [Crontab Guru](https://crontab.guru/) to read or check a cron expression.
