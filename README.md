# WindowsGSM - MeFriendos

WindowsGSM installs, updates and runs dedicated game servers on Windows, all from one app. This is the version we use for the [MeFriendos](https://mefriendos.de/) community servers. It builds on [Raziel7893's fork](https://github.com/Raziel7893/WindowsGSM) and adds our own server integrations and fixes.

[Download](#download) | [What's different](#whats-different-in-our-version) | [MeFriendos add-ons](#mefriendos-add-ons) | [Supported servers](#supported-servers) | [Docs](#documentation) | [Credits](#credits)

[![Latest release](https://img.shields.io/github/v/release/PapaGordon/WindowsGSM)](https://github.com/PapaGordon/WindowsGSM/releases/latest)
[![Build status](https://github.com/PapaGordon/WindowsGSM/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/PapaGordon/WindowsGSM/actions/workflows/build.yml)
[![MIT license](https://img.shields.io/github/license/PapaGordon/WindowsGSM.svg?label=license)](LICENSE)
[![Downloads](https://img.shields.io/github/downloads/PapaGordon/WindowsGSM/total.svg)](https://github.com/PapaGordon/WindowsGSM/releases)
[![MeFriendos Discord](https://img.shields.io/badge/Discord-MeFriendos-5865F2?logo=discord&logoColor=white)](https://discord.gg/vNJPTWY3UW)

## Download

Download the ZIP for Windows x64 from the [latest release](https://github.com/PapaGordon/WindowsGSM/releases/latest). `WindowsGSM.exe` already contains the .NET runtime, so there's nothing else to install. The ZIP also includes the docs and the helper for saving your Steam password encrypted.

- **New installation:** extract the ZIP into its own folder and start `WindowsGSM.exe`. Use **Install** for a new server or **Import** for one you already have.
- **Update:** close WindowsGSM, keep a copy of the old `WindowsGSM.exe` and replace it with the new one. Your `servers`, `configs`, `plugins` and `bin` folders stay as they are. Running game servers keep running while WindowsGSM is closed.

## What's different in our version

- **Workshop mods for DayZ and Arma 3** are updated before every start. If Steam can't be reached, the server still starts with the mods it has. [Workshop mods](docs/operations/workshop.md)
- **Steam passwords** can be saved encrypted for the Windows user that runs WindowsGSM. [Steam account and password](docs/operations/steam-credentials.md)
- **DayZ** keeps the server name, player slots and query port in its config in sync, lets you pick the mission and shows the server log inside WindowsGSM. [DayZ](docs/servers/dayz.md)
- **Minecraft** comes as Vanilla, NeoForge and ATM10. Vanilla can stay on one Minecraft release, NeoForge on one version family, and ATM10 keeps your extra mods when the pack updates.
- **Garry's Mod and other Source servers** keep their own console window and show their output in WindowsGSM as well. [Source console](docs/operations/source-console.md)
- **Auto Firewall** can be switched on or off per server and is applied whenever you change it and before the server starts.

Everything else from WindowsGSM is still there: backups and restores, automatic updates and restarts, scheduled commands, starting servers when you sign in, Discord alerts and remote control, CPU priority and affinity, and mod and plugin management.

## MeFriendos add-ons

These servers are built into our version. Just pick them under **Install** or **Import**. If you use a different WindowsGSM build, you can add them as standalone plugins instead. Each plugin has its own setup notes.

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

All guides are also listed in the [docs folder](docs/README.md).

**Setting up servers**

- [Built-in servers and what they need](docs/servers/built-in-servers.md)
- [Arma 3](docs/servers/arma3.md), [ATM10](docs/servers/atm10.md), [DayZ](docs/servers/dayz.md), [Enshrouded](docs/servers/enshrouded.md), [Farming Simulator 22 and 25](docs/servers/farming-simulator.md)
- [Minecraft Java Edition](docs/servers/minecraft-java.md), [Minecraft NeoForge](docs/servers/minecraft-neoforge.md), [Project Zomboid](docs/servers/project-zomboid.md), [SCP: Secret Laboratory](docs/servers/scp-secret-laboratory.md), [Valheim](docs/servers/valheim.md)

**Running servers**

- [Workshop mods for DayZ and Arma 3](docs/operations/workshop.md)
- [Steam account and password](docs/operations/steam-credentials.md)
- [Scheduled tasks with Crontab](docs/operations/crontab.md)
- [Console for Garry's Mod and other Source servers](docs/operations/source-console.md)

**Development**

- [Building and releasing](docs/development/build-and-release.md)
- [Changelog](CHANGELOG.md)
- [Changes inherited from Raziel7893](docs/development/upstream-changes.md)

For everything that works the same as in the original WindowsGSM, see the [WindowsGSM documentation](https://docs.windowsgsm.com/).

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
