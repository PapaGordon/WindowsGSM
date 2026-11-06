# Embedded console for Source servers

Garry's Mod and other Source servers run in their own Windows console. With **Embed Console** enabled, WindowsGSM shows their log output and sends commands to the native window.

1. Stop the server, turn on **Embed Console** and start it again.
2. Read the new output in WindowsGSM's console pane. Use **Toggle Console** to show or hide the native window.
3. Keep the default log filename. WindowsGSM adds `-condebug` and looks for `console.log` or `console.txt` in the game's folder.

If output is missing, check for a custom `con_logfile` setting and restore the default filename. Old entries stay in the log and are skipped when WindowsGSM starts following it. Stop the server before archiving the log.

This console handling first shipped in [v1.25.4.7](https://github.com/PapaGordon/WindowsGSM/releases/tag/v1.25.4.7) to fix a Garry's Mod startup crash with **Embed Console** enabled.
