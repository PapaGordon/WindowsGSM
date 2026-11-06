# Changes inherited from Raziel7893

The first MeFriendos release started from Raziel7893's WindowsGSM v1.25.2.2. These features came from that fork. [Raziel7893/WindowsGSM](https://github.com/Raziel7893/WindowsGSM)

## .NET, backups and schedules

- Raziel's fork moved WindowsGSM to .NET 8.
- Backups can include several paths. After the first start with backups enabled, open **Browse > Server Configs** to find `BackupConfig.cfg`.
- CSV schedules can run Windows commands and send commands to the game. [Crontab guide](../operations/crontab.md)

## Discord

- Join codes can appear in automatic start and restart alerts when **Embed Console** and **Send Join Codes** are enabled.
- The public IP webhook checks the address every two minutes and sends an alert when it changes.
- The `sendR` bot command sends a console command and tries to collect the response. It needs a working embedded console.
- Set a custom bot name and put an `avatar.png` in `configs/discordbot/` to use your own bot image. In the webhook settings, **Skip User Setup** lets the webhook use the name and avatar configured in Discord.
- Use bot user ID `0` to set default permissions for people who can use the bot's channel.

Later changes made for MeFriendos are listed in the [changelog](../../CHANGELOG.md).
