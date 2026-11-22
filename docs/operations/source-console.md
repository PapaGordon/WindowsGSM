# Console for Garry's Mod and other Source servers

Garry's Mod and other Source servers open their own console window. With **Embed Console** turned on, WindowsGSM shows that server's output in its own console pane, and the commands you type there go to the server.

## Turn it on

1. Stop the server.
2. Turn on **Embed Console** and start the server again.
3. Watch the output in the WindowsGSM console pane. **Toggle Console** shows or hides the server's own window.

WindowsGSM reads the output from the server's log file. To make that work, it starts the server with `-condebug` and follows `console.log` or `console.txt` in the game folder. Older lines already in the file are skipped.

## No output?

- Check whether your server config sets its own `con_logfile`. Remove it so the default file name is used again.
- If you want to archive or delete the log file, stop the server first.
