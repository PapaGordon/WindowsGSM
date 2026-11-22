# Scheduled tasks with Crontab

With Crontab files you can restart a server at fixed times, run a program or script on the host, or send a command to the running game, for example a restart warning.

## Set it up

1. Start the server once, so WindowsGSM creates the folder `servers/<server ID>/configs/Crontab/`. Only administrators can change files in it, because the tasks run programs on the host.
2. Select the server and open **Browse > Server Configs**. Put one or more `.csv` files into the `Crontab` folder.
3. Turn on **Restart Crontab** for the server.
4. Restart the server. WindowsGSM reads the files when the server starts, so restart it again after every change.

The restart time you enter in the WindowsGSM window is a separate schedule. The **Restart Crontab** switch turns on both, that one and your files.

## How a line looks

Each line starts with a schedule in the usual cron format (minute, hour, day, month, weekday), followed by what should happen. The parts are separated by semicolons. Lines starting with `//` are comments. Times are the local time of the Windows host.

| Action | What follows it |
| --- | --- |
| `restart` | Nothing. The server restarts. |
| `exec` | A program and its arguments. |
| `ServerConsoleCommand` | A command for the server console. |
| `RconCommand` | A command sent over RCON. |

[Crontab Guru](https://crontab.guru/) helps you read and check a schedule.

## Examples

Restart every day at 02:00 and run a backup script every day at 01:05:

```csv
0 2 * * *;restart
5 1 * * *;exec;cmd.exe;/C "C:\Scripts\backup.bat"
```

For ARK: warn the players, save the world and restart at 06:10:

```csv
5 6 * * *;ServerConsoleCommand;cheat serverchat Server will restart in 5 minutes
9 6 * * *;ServerConsoleCommand;cheat saveworld
10 6 * * *;restart
```

## Commands over RCON

1. Turn on RCON in the game server's own settings.
2. Close WindowsGSM and add the RCON address, port and password to the server's `configs/WindowsGSM.cfg`. Use the address the game really listens on. If the server already has an RCON password, use that one.

```ini
rconip="127.0.0.1"
rconport="25575"
rconpassword="YOUR_RCON_PASSWORD"
```

3. Start WindowsGSM again. This line now sends a message five minutes past every hour:

```csv
5 * * * *;RconCommand;say This message runs every hour
```

The server's answer appears in the WindowsGSM log. If the RCON port is missing in `WindowsGSM.cfg`, the log says so instead.

Some games speak their own RCON dialect. If the game's own RCON tool works but WindowsGSM reports a timeout, run that tool with `exec` instead.

## Good to know

- Tasks only run while the server is running.
- Leave enough time between tasks that depend on each other.
- Use commands that finish on their own. WindowsGSM doesn't stop them and they keep running until the server restarts.
- Programs started with `exec` run as the Windows user that runs WindowsGSM. Their output is saved in `logs/Server_<server ID>_<program>_execLog.log`.
