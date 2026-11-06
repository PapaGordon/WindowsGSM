# Built-in MeFriendos servers

Pick a server from **Install** or **Import** in the MeFriendos build. The standalone plugins are for people using another compatible WindowsGSM build. [MeFriendos add-ons](../../README.md#mefriendos-add-ons)

## Existing servers

WindowsGSM recognizes old plugin entries such as `Enshrouded Dedicated Server [Enshrouded.cs]` and runs them with the built-in server module. There is no need to edit the server config or import it again. Installed plugin copies still appear in plugin management, while **Install** and **Import** show a single entry for the game.

DayZ uses the existing **DayZ Dedicated Server** entry and recognizes old `DayZ Dedicated Server [DayZ.cs]` entries too. Its built-in module includes the settings, console and Workshop handling from our standalone DayZ plugin. [DayZ setup](dayz.md)

## Game requirements

- **ATM10** needs the CurseForge server pack. Put the server ZIP in the WindowsGSM root or in `serverfiles`.
- **Minecraft NeoForge** starts with the loader and an empty mod list. Add the server mods you want to run. [NeoForge setup](minecraft-neoforge.md)
- **Farming Simulator 22 and 25** need the matching game license and dedicated server setup.
- **Valheim** needs a new server password in place of the default placeholder.

For DayZ and Arma 3 mods, see [Workshop updates](../operations/workshop.md). Servers using your Steam account can use the [encrypted password setup](../operations/steam-credentials.md).

Auto Firewall covers the game processes started by the Farming Simulator managers and SCP:SL LocalAdmin. ATM10 uses a TCP game rule and a UDP query rule named for each server instance.

## Imported code

The imported modules live in `WindowsGSM/GameServer/Bundled` and use the `WindowsGSM.GameServer.Bundled` namespace. Keep the standalone copies in sync when changing shared server behavior. NeoForge uses ATM10's Java runtime helpers and has its own installer and update flow.

The table records the initial imports. The original plugin notices are embedded in the assembly and kept in [licenses](../../licenses/). See the [changelog](../../CHANGELOG.md) for later changes.

| Server | Standalone repository | Original import revision |
| --- | --- | --- |
| Arma 3 Dedicated Server | [WindowsGSM.ARMA3-MeFriendos](https://github.com/PapaGordon/WindowsGSM.ARMA3) | `76f58baade48a349328459d5b65420a3294ec9df` |
| Minecraft: All the Mods 10 (ATM10) | [WindowsGSM.ATM10-MeFriendos](https://github.com/PapaGordon/WindowsGSM.ATM10) | `e372350094ee2b09c56fa8ee2fe2f8213278be91` |
| Enshrouded Dedicated Server | [WindowsGSM.Enshrouded](https://github.com/PapaGordon/WindowsGSM.Enshrouded) | `57e7d5c5a783a4973f767a1c0d2f6c4adeea8c4f` |
| Farming Simulator 22 Dedicated Server | [WindowsGSM.FarmingSimulator22](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator22) | `ee7459d100f46d9491079a5f15cd720e344f6a5d` |
| Farming Simulator 25 Dedicated Server | [WindowsGSM.FarmingSimulator25](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator25) | `3e988c7742180266b7691b699c7d1c294b6d6c67` |
| Project Zomboid Dedicated Server | [WindowsGSM.ProjectZomboid-MeFriendos](https://github.com/PapaGordon/WindowsGSM.ProjectZomboid) | `7915c73255dbd8edcb84faa57ba5dc754d24b9fc` |
| SCP: Secret Laboratory Dedicated Server | [WindowsGSM.SCPSL-MeFriendos](https://github.com/PapaGordon/WindowsGSM.SCPSL) | `8a207683cc3f363caab101c7db150e16112ac159` |
| Valheim Dedicated Server | [WindowsGSM.Valheim-MeFriendos](https://github.com/PapaGordon/WindowsGSM.Valheim) | `3536aa02111167377f8864eb050cee400df0978f` |
