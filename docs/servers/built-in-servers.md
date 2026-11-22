# Built-in MeFriendos servers

Besides the games from the original WindowsGSM, our build contains the servers below. Pick them under **Install** or **Import**, no plugin needed. We publish the same servers as standalone plugins for people who use another WindowsGSM build. [Overview with plugin links](../../README.md#mefriendos-add-ons)

| Game | Guide | Good to know before the first start |
| --- | --- | --- |
| Arma 3 | [Arma 3](arma3.md) | Needs a Steam account. Max Players and Map are set in `server.cfg`. |
| ATM10 | [ATM10](atm10.md) | Needs the `ServerFiles-<version>.zip` from CurseForge and Java 21. |
| DayZ | [DayZ](dayz.md) | Needs a Steam account. |
| Enshrouded | [Enshrouded](enshrouded.md) | Creates separate passwords for four player roles. |
| Farming Simulator 22 and 25 | [Farming Simulator](farming-simulator.md) | The Steam account has to own the game. Settings are made in the GIANTS web panel. |
| Minecraft Java Edition | [Minecraft Vanilla](minecraft-java.md) | Needs the matching Java version. |
| Minecraft NeoForge | [NeoForge](minecraft-neoforge.md) | Starts without mods. Needs the matching Java version. |
| Project Zomboid | [Project Zomboid](project-zomboid.md) | Always starts with 16 GB RAM. Asks for an admin password on the first start. |
| SCP: Secret Laboratory | [SCP: Secret Laboratory](scp-secret-laboratory.md) | You accept Northwood's EULA yourself in LocalAdmin. |
| Valheim | [Valheim](valheim.md) | Replace the `CHANGE_ME` password before the first start. |

The other games, such as Garry's Mod, Rust or Counter-Strike, work the same as in the original WindowsGSM. They're listed in the [README](../../README.md#supported-servers), and the [WindowsGSM documentation](https://docs.windowsgsm.com/) covers them. For Garry's Mod and other Source servers we improved the console. [Source console](../operations/source-console.md)

## Servers you set up with a plugin

You don't need to change anything. Servers that were installed with one of our plugins, for example as `Enshrouded Dedicated Server [Enshrouded.cs]`, are picked up by the built-in version automatically. There's no need to import them again or edit their config. The plugin still shows up in plugin management, but **Install** and **Import** only list the game once.

DayZ works the same way: old `DayZ Dedicated Server [DayZ.cs]` entries run with the built-in **DayZ Dedicated Server**, which includes everything from our DayZ plugin. [DayZ setup](dayz.md)

## Steam account, mods and firewall

- **Steam account:** DayZ, Arma 3 and Farming Simulator download with a Steam account. [Steam account and password](../operations/steam-credentials.md)
- **Workshop mods:** DayZ and Arma 3 can keep a list of Workshop mods up to date. [Workshop mods](../operations/workshop.md)
- **Auto Firewall:** For most games, WindowsGSM allows the server's program in the Windows firewall. For DayZ, Farming Simulator and SCP:SL it allows both programs involved. For Minecraft, NeoForge and ATM10 it creates a TCP rule for the game port and a UDP rule for the query port instead, so Java isn't allowed in general. Router port forwarding is always up to you.

## Where the built-in servers come from

The code lives in `WindowsGSM/GameServer/Bundled` (namespace `WindowsGSM.GameServer.Bundled`). When we change how one of these servers behaves, we update the standalone plugin too. NeoForge shares the Java helpers with ATM10 but has its own installer and updates.

The table shows the plugin version each server was first imported from. The original license notices are kept in [licenses](../../licenses/). Later changes are in the [changelog](../../CHANGELOG.md).

| Server | Standalone plugin | First imported from |
| --- | --- | --- |
| Arma 3 Dedicated Server | [WindowsGSM.ARMA3](https://github.com/PapaGordon/WindowsGSM.ARMA3) | `76f58baade48a349328459d5b65420a3294ec9df` |
| Minecraft: All the Mods 10 (ATM10) | [WindowsGSM.ATM10](https://github.com/PapaGordon/WindowsGSM.ATM10) | `e372350094ee2b09c56fa8ee2fe2f8213278be91` |
| Enshrouded Dedicated Server | [WindowsGSM.Enshrouded](https://github.com/PapaGordon/WindowsGSM.Enshrouded) | `57e7d5c5a783a4973f767a1c0d2f6c4adeea8c4f` |
| Farming Simulator 22 Dedicated Server | [WindowsGSM.FarmingSimulator22](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator22) | `ee7459d100f46d9491079a5f15cd720e344f6a5d` |
| Farming Simulator 25 Dedicated Server | [WindowsGSM.FarmingSimulator25](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator25) | `3e988c7742180266b7691b699c7d1c294b6d6c67` |
| Project Zomboid Dedicated Server | [WindowsGSM.ProjectZomboid](https://github.com/PapaGordon/WindowsGSM.ProjectZomboid) | `7915c73255dbd8edcb84faa57ba5dc754d24b9fc` |
| SCP: Secret Laboratory Dedicated Server | [WindowsGSM.SCPSL](https://github.com/PapaGordon/WindowsGSM.SCPSL) | `8a207683cc3f363caab101c7db150e16112ac159` |
| Valheim Dedicated Server | [WindowsGSM.Valheim](https://github.com/PapaGordon/WindowsGSM.Valheim) | `3536aa02111167377f8864eb050cee400df0978f` |
