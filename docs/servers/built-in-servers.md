# Built-in MeFriendos servers

Our WindowsGSM build already contains Arma 3, ATM10, DayZ, Enshrouded, Farming Simulator 22 and 25, Minecraft NeoForge, Project Zomboid, SCP: Secret Laboratory and Valheim. Pick the game under **Install** or **Import**, no plugin needed. We publish the same servers as standalone plugins for people who use another WindowsGSM build. [Overview with plugin links](../../README.md#mefriendos-add-ons)

## Servers you set up with a plugin

You don't need to change anything. Servers that were installed with one of our plugins, for example as `Enshrouded Dedicated Server [Enshrouded.cs]`, are picked up by the built-in version automatically. There's no need to import them again or edit their config. The plugin still shows up in plugin management, but **Install** and **Import** only list the game once.

DayZ works the same way: old `DayZ Dedicated Server [DayZ.cs]` entries run with the built-in **DayZ Dedicated Server**, which includes everything from our DayZ plugin. [DayZ setup](dayz.md)

## What some games need

- **ATM10** needs the server pack from CurseForge. Download `ServerFiles-<version>.zip` and put it into the WindowsGSM folder or the server's `serverfiles` folder before you click **Install** or **Update**.
- **Minecraft NeoForge** starts with the loader and no mods. Add the mods you want yourself. [NeoForge setup](minecraft-neoforge.md)
- **Farming Simulator 22 and 25** are installed through Steam together with the game, so the saved Steam account has to own it.
- **Valheim** needs a server password of at least 5 characters. Replace `CHANGE_ME` after `-password` in the start parameters before the first start.
- **SCP: Secret Laboratory** asks you to accept Northwood's EULA in LocalAdmin. WindowsGSM doesn't do that for you on purpose.
- **DayZ, Arma 3 and Farming Simulator** download with a Steam account. [Steam account and password](../operations/steam-credentials.md)

DayZ and Arma 3 can also keep a list of Workshop mods up to date. [Workshop mods](../operations/workshop.md)

**Auto Firewall** also covers the processes that the Farming Simulator manager and SCP:SL LocalAdmin start. ATM10 gets one TCP rule for the game and one UDP rule for the query port per server.

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
