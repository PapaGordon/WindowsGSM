# WindowsGSM - MeFriendos

This is the MeFriendos fork of WindowsGSM. Use it to install, import and run dedicated game servers from one Windows app. We started from [Raziel7893's fork](https://github.com/Raziel7893/WindowsGSM).

[Download](#download) | [Fork features](#fork-features) | [MeFriendos add-ons](#mefriendos-add-ons) | [Supported servers](#supported-servers) | [Docs](#documentation) | [Credits](#credits)

[![Latest release](https://img.shields.io/github/v/release/PapaGordon/WindowsGSM)](https://github.com/PapaGordon/WindowsGSM/releases/latest)
[![Build status](https://github.com/PapaGordon/WindowsGSM/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/PapaGordon/WindowsGSM/actions/workflows/build.yml)
[![MIT license](https://img.shields.io/github/license/PapaGordon/WindowsGSM.svg?label=license)](LICENSE)
[![Downloads](https://img.shields.io/github/downloads/PapaGordon/WindowsGSM/total.svg)](https://github.com/PapaGordon/WindowsGSM/releases)
[![MeFriendos Discord](https://img.shields.io/badge/Discord-MeFriendos-5865F2?logo=discord&logoColor=white)](https://discord.gg/vNJPTWY3UW)

## Download

Get the Windows x64 ZIP from the [latest MeFriendos release](https://github.com/PapaGordon/WindowsGSM/releases/latest). The .NET runtime is bundled in `WindowsGSM.exe`. The ZIP also includes the Steam password setup helper.

- For a fresh install, extract the ZIP to its own folder and run `WindowsGSM.exe`. Use **Install** for a new server or **Import** for an existing one.
- To update, close WindowsGSM and back up your old `WindowsGSM.exe` before replacing it. Keep your `servers`, `configs`, `plugins` and `bin` folders.

## Fork features

- **Auto Firewall** can be turned on or off for each server. WindowsGSM saves the setting and applies it when you change the switch and before the server starts.
- **DayZ** syncs the server name, player slots and query port with the server config. You can pick the mission and view the RPT log in WindowsGSM.
- **Workshop updates** can download configured mods for DayZ and Arma 3 before each start.
- **Minecraft** has separate entries for Vanilla, NeoForge and ATM10. Vanilla can pin a Mojang release, NeoForge can stay on a Minecraft version family, and ATM10 stages pack updates while keeping extra mods with separate mod IDs.
- **Source servers** such as Garry's Mod keep their native console and show new lines from the game log in **Embed Console**.
- **Steam passwords** can be saved encrypted for the Windows account that runs WindowsGSM.

You can also back up and restore server files, set up automatic updates and restarts, schedule commands, and start servers on login. WindowsGSM has Discord alerts and remote control, CPU priority and affinity settings, and tools for managing mods and plugins.

## MeFriendos add-ons

Every server in this table is already built into our fork. Select it under **Install** or **Import**. We also publish the plugins separately for anyone who uses a different compatible WindowsGSM build. Each plugin repository has its own setup steps and requirements.

| | Built-in server | Query | Standalone plugin |
| --- | --- | --- | --- |
| <img src="WindowsGSM/Images/Games/arma3.png?raw=true" alt="" height="24" /> | Arma 3 Dedicated Server | A2S | [Plugin](https://github.com/PapaGordon/WindowsGSM.ARMA3) · [Download](https://github.com/PapaGordon/WindowsGSM.ARMA3/releases/latest) |
| <img src="WindowsGSM/Images/Games/dayz.png?raw=true" alt="" height="24" /> | DayZ Dedicated Server | A2S | [Plugin](https://github.com/PapaGordon/WindowsGSM.DayZ) · [Download](https://github.com/PapaGordon/WindowsGSM.DayZ/releases/latest) |
| <img src="WindowsGSM/Images/Games/enshrouded.png?raw=true" alt="" height="24" /> | Enshrouded Dedicated Server | A2S | [Plugin](https://github.com/PapaGordon/WindowsGSM.Enshrouded) · [Download](https://github.com/PapaGordon/WindowsGSM.Enshrouded/releases/latest) |
| <img src="WindowsGSM/Images/Games/fs22.png?raw=true" alt="" height="24" /> | Farming Simulator 22 Dedicated Server | | [Plugin](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator22) · [Download](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator22/releases/latest) |
| <img src="WindowsGSM/Images/Games/fs25.png?raw=true" alt="" height="24" /> | Farming Simulator 25 Dedicated Server | | [Plugin](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator25) · [Download](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator25/releases/latest) |
| <img src="WindowsGSM/Images/Games/atm10.png?raw=true" alt="" height="24" /> | Minecraft: All the Mods 10 (ATM10) | UT3 | [Plugin](https://github.com/PapaGordon/WindowsGSM.ATM10) · [Download](https://github.com/PapaGordon/WindowsGSM.ATM10/releases/latest) |
| <img src="WindowsGSM/Images/Games/neoforged.ico?raw=true" alt="" height="24" /> | Minecraft: NeoForge | UT3 | [Plugin](https://github.com/PapaGordon/WindowsGSM.Minecraft_NeoForge) · [Download](https://github.com/PapaGordon/WindowsGSM.Minecraft_NeoForge/releases/latest) |
| <img src="WindowsGSM/Images/Games/pz.png?raw=true" alt="" height="24" /> | Project Zomboid Dedicated Server | A2S | [Plugin](https://github.com/PapaGordon/WindowsGSM.ProjectZomboid) · [Download](https://github.com/PapaGordon/WindowsGSM.ProjectZomboid/releases/latest) |
| <img src="WindowsGSM/Images/Games/scpsl.png?raw=true" alt="" height="24" /> | SCP: Secret Laboratory Dedicated Server | | [Plugin](https://github.com/PapaGordon/WindowsGSM.SCPSL) · [Download](https://github.com/PapaGordon/WindowsGSM.SCPSL/releases/latest) |
| <img src="WindowsGSM/Images/Games/valheim.png?raw=true" alt="" height="24" /> | Valheim Dedicated Server | A2S | [Plugin](https://github.com/PapaGordon/WindowsGSM.Valheim) · [Download](https://github.com/PapaGordon/WindowsGSM.Valheim/releases/latest) |

## Supported servers

<details>
<summary>Other built-in servers</summary>

| | Built-in server | Query |
| --- | --- | --- |
| <img src="WindowsGSM/Images/Games/7dtd.png?raw=true" alt="" height="24" /> | 7 Days to Die Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/arkse.png?raw=true" alt="" height="24" /> | ARK: Survival Evolved Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/avorion.png?raw=true" alt="" height="24" /> | Avorion Dedicated Server | |
| <img src="WindowsGSM/Images/Games/bt.png?raw=true" alt="" height="24" /> | Barotrauma Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/bw.png?raw=true" alt="" height="24" /> | BlackWake Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/ce.png?raw=true" alt="" height="24" /> | Conan Exiles Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/cs.png?raw=true" alt="" height="24" /> | Counter-Strike: 1.6 Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/cscz.png?raw=true" alt="" height="24" /> | Counter-Strike: Condition Zero Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/csgo.png?raw=true" alt="" height="24" /> | Counter-Strike: Global Offensive Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/css.png?raw=true" alt="" height="24" /> | Counter-Strike: Source Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/dod.png?raw=true" alt="" height="24" /> | Day of Defeat Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/dods.png?raw=true" alt="" height="24" /> | Day of Defeat: Source Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/dmc.png?raw=true" alt="" height="24" /> | Deathmatch Classic Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/eco.png?raw=true" alt="" height="24" /> | Eco Dedicated Server | |
| <img src="WindowsGSM/Images/Games/egs.png?raw=true" alt="" height="24" /> | Empyrion - Galactic Survival Dedicated Server | |
| <img src="WindowsGSM/Images/Games/gmod.png?raw=true" alt="" height="24" /> | Garry's Mod Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/gta5.png?raw=true" alt="" height="24" /> | Grand Theft Auto V Dedicated Server (FiveM) | FIVEM |
| <img src="WindowsGSM/Images/Games/hl2dm.png?raw=true" alt="" height="24" /> | Half-Life 2: Deathmatch Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/hlof.png?raw=true" alt="" height="24" /> | Half-Life: Opposing Force Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/heat.png?raw=true" alt="" height="24" /> | Heat Dedicated Server | |
| <img src="WindowsGSM/Images/Games/ins.png?raw=true" alt="" height="24" /> | Insurgency Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/inss.png?raw=true" alt="" height="24" /> | Insurgency: Sandstorm Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/l4d2.png?raw=true" alt="" height="24" /> | Left 4 Dead 2 Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/mcbe.png?raw=true" alt="" height="24" /> | Minecraft: Bedrock Edition Server | |
| <img src="WindowsGSM/Images/Games/mc.png?raw=true" alt="" height="24" /> | Minecraft: Java Edition Server | UT3 |
| <img src="WindowsGSM/Images/Games/mcpe.png?raw=true" alt="" height="24" /> | Minecraft: Pocket Edition Server (PocketMine-MP) | UT3 |
| <img src="WindowsGSM/Images/Games/mordhau.png?raw=true" alt="" height="24" /> | Mordhau Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/nmrih.png?raw=true" alt="" height="24" /> | No More Room in Hell Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/onset.png?raw=true" alt="" height="24" /> | Onset Dedicated Server | |
| <img src="WindowsGSM/Images/Games/olow.png?raw=true" alt="" height="24" /> | Outlaws of the Old West Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/ps.png?raw=true" alt="" height="24" /> | Post Scriptum Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/rok.png?raw=true" alt="" height="24" /> | Reign Of Kings Dedicated Server | |
| <img src="WindowsGSM/Images/Games/rcc.png?raw=true" alt="" height="24" /> | Ricochet Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/ror2.png?raw=true" alt="" height="24" /> | Risk of Rain 2 Dedicated Server | |
| <img src="WindowsGSM/Images/Games/rust.png?raw=true" alt="" height="24" /> | Rust Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/hl2dm.png?raw=true" alt="" height="24" /> | Source SDK Base 2013 Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/se.png?raw=true" alt="" height="24" /> | Space Engineers Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/sq.png?raw=true" alt="" height="24" /> | Squad Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/sw.png?raw=true" alt="" height="24" /> | Stormworks Dedicated Server | |
| <img src="WindowsGSM/Images/Games/tf2.png?raw=true" alt="" height="24" /> | Team Fortress 2 Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/tfc.png?raw=true" alt="" height="24" /> | Team Fortress Classic Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/tf.png?raw=true" alt="" height="24" /> | The Forest Dedicated Server | |
| <img src="WindowsGSM/Images/Games/unt.png?raw=true" alt="" height="24" /> | Unturned Dedicated Server | A2S |
| <img src="WindowsGSM/Images/Games/vts.png?raw=true" alt="" height="24" /> | Vintage Story Dedicated Server | |
| <img src="WindowsGSM/Images/Games/zps.png?raw=true" alt="" height="24" /> | Zombie Panic Source Dedicated Server | A2S |

</details>

For more games, see the [community plugins in the original WindowsGSM README](https://github.com/WindowsGSM/WindowsGSM#plugins), including plugins by BattlefieldDuck and GTVolk.

## Documentation

[Browse the docs folder](docs/README.md)

### Server setup

- [Built-in servers and their requirements](docs/servers/built-in-servers.md)
- [DayZ settings, missions and console](docs/servers/dayz.md)
- [Vanilla Minecraft setup](docs/servers/minecraft-java.md)
- [Minecraft NeoForge setup](docs/servers/minecraft-neoforge.md)

### Running and updating servers

- [Workshop updates for DayZ and Arma 3](docs/operations/workshop.md)
- [Steam password encryption](docs/operations/steam-credentials.md)
- [Scheduling commands with Crontab](docs/operations/crontab.md)
- [Embedded console for Garry's Mod and other Source servers](docs/operations/source-console.md)

### Development and releases

- [Build and release instructions](docs/development/build-and-release.md)
- [MeFriendos changelog](CHANGELOG.md)
- [Changes inherited from Raziel7893](docs/development/upstream-changes.md)

For general WindowsGSM help, see the [original WindowsGSM documentation](https://docs.windowsgsm.com/).

## Community

Find us on [MeFriendos Discord](https://discord.gg/vNJPTWY3UW) and at [mefriendos.de](https://mefriendos.de/). For bugs in our fork, use [GitHub Issues](https://github.com/PapaGordon/WindowsGSM/issues).

The original WindowsGSM community is on [Discord](https://discord.gg/bGc7t2R) and [Steam](https://steamcommunity.com/groups/WindowsGSM).

## Credits

### MeFriendos

PapaGordon maintains this fork, the server integrations and the standalone plugins. Thanks to everyone who tries our builds, shares logs and helps us track down bugs.

### WindowsGSM and upstream

- **TatLead (BattlefieldDuck)** and the original contributors made [WindowsGSM](https://github.com/WindowsGSM/WindowsGSM).
- **Raziel7893** and the contributors to the parent fork brought in .NET 8 and improved backups, crontab and Discord webhooks.
- **Deathreus** helped optimize the code. **! AssaultLine** and **Dennis** tested the Mordhau server.

See [CREDITS.md](CREDITS.md) for the full credits, including plugin authors, libraries and game artwork.

## License

The code uses the [MIT License](LICENSE), which includes the original and fork copyright notices. See [licenses](licenses/) for the notices for imported code and artwork.

## Stargazers over time

[![Star history for PapaGordon/WindowsGSM](https://api.star-history.com/svg?repos=PapaGordon/WindowsGSM&type=Date)](https://www.star-history.com/#PapaGordon/WindowsGSM&Date)
