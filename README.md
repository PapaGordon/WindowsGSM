# WindowsGSM - MeFriendos

Our WindowsGSM build, based on [Raziel7893/WindowsGSM](https://github.com/Raziel7893/WindowsGSM).

[Download the latest release](https://github.com/PapaGordon/WindowsGSM/releases/latest) | [Changelog](CHANGELOG.md) | [Development](DEVELOPMENT.md)

The Windows x64 release includes the .NET runtime. Stop WindowsGSM before replacing `WindowsGSM.exe`. Keep your existing server, plugin and configuration folders.

This build includes a per-server **Auto Firewall** switch. Its state is saved for each server and applied when you change the switch and before startup. Turn it off if you manage the server's port rules yourself.

Arma 3, ATM10, Enshrouded, Farming Simulator 22/25, Project Zomboid, SCP: Secret Laboratory and Valheim are built in. They appear in Install and Import without an extra plugin download. The [standalone plugins](BUNDLED-SERVERS.md) remain available for other WindowsGSM builds. Existing server configurations with our old plugin names still work.

DayZ uses the existing built-in entry, with synchronized name, slots and query port, mission selection and local default configuration. See [DayZ setup](DAYZ.md).

Our development branch is `main`. The existing upstream pull request stays on `master`; normal pushes to `main` do not update that request. Upstream changes can still be fetched and merged when needed.

---

![Screenshot](https://windowsgsm.com/carousel/windowsgsm-desktop.png)

[![Open Source](https://badges.frapsoft.com/os/v1/open-source.svg?v=103)](https://github.com/PapaGordon/WindowsGSM)
[![Build status](https://github.com/PapaGordon/WindowsGSM/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/PapaGordon/WindowsGSM/actions/workflows/build.yml)
[![GitHub license](https://img.shields.io/github/license/PapaGordon/WindowsGSM.svg)](https://github.com/PapaGordon/WindowsGSM/blob/main/LICENSE)
[![GitHub release](https://img.shields.io/github/release/PapaGordon/WindowsGSM.svg)](https://github.com/PapaGordon/WindowsGSM/releases/)
[![Github all releases](https://img.shields.io/github/downloads/PapaGordon/WindowsGSM/total.svg)](https://github.com/PapaGordon/WindowsGSM/releases/)
[![MeFriendos Discord](https://img.shields.io/badge/Discord-MeFriendos-5865F2?logo=discord&logoColor=white)](https://discord.gg/vNJPTWY3UW)

## Changelog

Our changes are in [CHANGELOG.md](CHANGELOG.md). Earlier changes from the Raziel fork are in [ChangesFromBase.md](ChangesFromBase.md).

# What is WindowsGSM?
WindowsGSM is a powerful tool to manage game servers. Equipped with a GUI for server admins to install, import, start, stop, restart, update, and automate multiple servers with a push of a button.

|               | 🎮 Game Server  | Supported | Query |
| ------------- | --------------- | --------- | ----- |
| ![logo](WindowsGSM/Images/Games/arma3.png?raw=true) | Arma 3 Dedicated Server |✅| A2S |
| ![logo](WindowsGSM/Images/Games/atm10.png?raw=true) | Minecraft: All the Mods 10 (ATM10) |✅| UT3 |
| ![logo](WindowsGSM/Images/Games/enshrouded.png?raw=true) | Enshrouded Dedicated Server |✅| A2S |
| ![logo](WindowsGSM/Images/Games/fs22.png?raw=true) | Farming Simulator 22 Dedicated Server |✅| |
| ![logo](WindowsGSM/Images/Games/fs25.png?raw=true) | Farming Simulator 25 Dedicated Server |✅| |
| ![logo](WindowsGSM/Images/Games/pz.png?raw=true) | Project Zomboid Dedicated Server |✅| A2S |
| ![logo](WindowsGSM/Images/Games/scpsl.png?raw=true) | SCP: Secret Laboratory Dedicated Server |✅| |
| ![logo](WindowsGSM/Images/Games/valheim.png?raw=true) | Valheim Dedicated Server |✅| A2S |
| ![logo](WindowsGSM/Images/Games/7dtd.png?raw=true)   | 7 Days to Die Dedicated Server                    |✅|      |
| ![logo](WindowsGSM/Images/Games/arkse.png?raw=true)  | ARK: Survival Evolved Dedicated Server            |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/avorion.png?raw=true)| Avorion Dedicated Server                          |✅|      |
| ![logo](WindowsGSM/Images/Games/bt.png?raw=true)     | Barotrauma Dedicated Server                       |✅|      |
| ![logo](WindowsGSM/Images/Games/bw.png?raw=true)     | BlackWake Dedicated Server                        |✅|      |
| ![logo](WindowsGSM/Images/Games/ce.png?raw=true)     | Conan Exiles Dedicated Server                     |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/cs.png?raw=true)     | Counter-Strike: 1.6 Dedicated Server              |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/cscz.png?raw=true)   | Counter-Strike: Condition Zero Dedicated Server   |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/csgo.png?raw=true)   | Counter-Strike: Global Offensive Dedicated Server |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/css.png?raw=true)    | Counter-Strike: Source Dedicated Server           |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/dod.png?raw=true)    | Day of Defeat Dedicated Server                    |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/dods.png?raw=true)   | Day of Defeat: Source Dedicated Server            |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/dayz.png?raw=true)   | DayZ Dedicated Server                             |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/dmc.png?raw=true)    | Deathmatch Classic Dedicated Server               |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/eco.png?raw=true)    | Eco Dedicated Server                              |✅|      |
| ![logo](WindowsGSM/Images/Games/egs.png?raw=true)    | Empyrion - Galactic Survival Dedicated Server     |✅|      |
| ![logo](WindowsGSM/Images/Games/gmod.png?raw=true)   | Garry's Mod Dedicated Server                      |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/gta5.png?raw=true)   | Grand Theft Auto V Dedicated Server ([FiveM](https://github.com/citizenfx/fivem))|✅| FIVEM |
| ![logo](WindowsGSM/Images/Games/hl2dm.png?raw=true)  | Half-Life 2: Deathmatch Dedicated Server          |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/hlof.png?raw=true)   | Half-Life: Opposing Force Dedicated Server        |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/heat.png?raw=true)   | Heat Dedicated Server                             |✅|      |
| ![logo](WindowsGSM/Images/Games/ins.png?raw=true)    | Insurgency Dedicated Server                       |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/inss.png?raw=true)   | Insurgency: Sandstorm Dedicated Server            |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/l4d2.png?raw=true)   | Left 4 Dead 2 Dedicated Server                    |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/mcbe.png?raw=true)   | Minecraft: Bedrock Edition Server                 |✅|      |
| ![logo](WindowsGSM/Images/Games/mc.png?raw=true)     | Minecraft: Java Edition Server                    |✅| UT3  |
| ![logo](WindowsGSM/Images/Games/mcpe.png?raw=true)   | Minecraft: Pocket Edition Server ([PocketMine-MP](https://github.com/pmmp/PocketMine-MP)) |✅| UT3  |
| ![logo](WindowsGSM/Images/Games/mordhau.png?raw=true)| Mordhau Dedicated Server                          |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/nmrih.png?raw=true)  | No More Room in Hell Dedicated Server             |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/onset.png?raw=true)  | Onset Dedicated Server                            |✅|      |
| ![logo](WindowsGSM/Images/Games/olow.png?raw=true)   | Outlaws of the Old West Dedicated Server          |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/ps.png?raw=true)     | Post Scriptum Dedicated Server                    |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/rok.png?raw=true)    | Reign Of Kings Dedicated Server                   |✅|      |
| ![logo](WindowsGSM/Images/Games/rcc.png?raw=true)    | Ricochet Dedicated Server                         |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/rcc.png?raw=true)    | Risk of Rain 2 Dedicated Server                   |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/rust.png?raw=true)   | Rust Dedicated Server                             |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/se.png?raw=true)     | Space Engineers Dedicated Server                  |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/sq.png?raw=true)     | Squad Dedicated Server                            |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/sw.png?raw=true)     | Stormworks Dedicated Server                       |✅|      |
| ![logo](WindowsGSM/Images/Games/tf2.png?raw=true)    | Team Fortress 2 Dedicated Server                  |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/tfc.png?raw=true)    | Team Fortress Classic Dedicated Server            |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/tf.png?raw=true)     | The Forest Dedicated Server                       |✅|      |
| ![logo](WindowsGSM/Images/Games/unt.png?raw=true)    | Unturned Dedicated Server                         |✅| A2S  |
| ![logo](WindowsGSM/Images/Games/zps.png?raw=true)    | Zombie Panic Source Dedicated Server              |✅| A2S  |

|               | 🎮 Game Server  | Plugin supported | Query |
| ------------- | --------------- | --------- | ----- |
| ![logo](WindowsGSM/Images/Games/arma3.png?raw=true) | Arma 3 Dedicated Server |✅| A2S |
| ![logo](WindowsGSM/Images/Games/atm10.png?raw=true) | Minecraft: All the Mods 10 (ATM10) |✅| UT3 |
| ![logo](WindowsGSM/Images/Games/enshrouded.png?raw=true) | Enshrouded Dedicated Server |✅| A2S |
| ![logo](WindowsGSM/Images/Games/fs22.png?raw=true) | Farming Simulator 22 Dedicated Server |✅| |
| ![logo](WindowsGSM/Images/Games/fs25.png?raw=true) | Farming Simulator 25 Dedicated Server |✅| |
| ![logo](WindowsGSM/Images/Games/pz.png?raw=true) | Project Zomboid Dedicated Server |✅| A2S |
| ![logo](WindowsGSM/Images/Games/scpsl.png?raw=true) | SCP: Secret Laboratory Dedicated Server |✅| |
| ![logo](WindowsGSM/Images/Games/valheim.png?raw=true) | Valheim Dedicated Server |✅| A2S |
| <img src="https://github.com/DevVault/WindowsGSM.ARMA2/blob/master/ARMA2.cs/ARMA2.png?raw=true" width="16" height="16"/>   | Arma2   |✅| A2S     |
| <img src="https://github.com/DevVault/WindowsGSM.ARMA2/blob/master/ARMA2OA.cs/ARMA2OA.png?raw=true" width="16" height="16"/>   | Arma2: OA   |✅| A2S     |
| ![logo](https://github.com/1stian/imgrepo/blob/master/WindowsGSMrepo/arma3.png?raw=true)   | Arma3   |✅|      |
| ![logo](https://github.com/1stian/imgrepo/blob/master/WindowsGSMrepo/astroneer.png?raw=true)   | ASTRONEER   |✅|      |
| ![logo](https://github.com/1stian/imgrepo/blob/master/WindowsGSMrepo/deadmatter.png?raw=true)   | Dead Matter   |✅| UT3     |
| ![logo](https://github.com/1stian/imgrepo/blob/master/WindowsGSMrepo/papermc.png?raw=true)   | PaperMC   |✅| A2S     |
| <img src="https://github.com/DevVault/WindowsGSM.HLDM/blob/master/HLDM.cs/HLDM.png?raw=true" width="16" height="16"/>   | Half-Life: Deathmatch   |✅| A2S     |
| <img src="https://github.com/DevVault/WindowsGSM.L4D/blob/master/L4D.cs/L4D.png?raw=true" width="16" height="16"/>   | Left 4 Dead   |✅| A2S     |
| ![logo](https://github.com/1stian/imgrepo/blob/master/WindowsGSMrepo/waterfall.png?raw=true)   | PaperMC: Waterfall   |✅| A2S     |
| ![logo](https://github.com/1stian/imgrepo/blob/master/WindowsGSMrepo/spigot.png?raw=true)   | Spigot   |✅| A2S     |

## Features
  * **Easy Install** - No installation knowledge is required, download any game server by one-click install.
  * **Manage multiple servers** - Manage multiple servers with the same software. No more messy server files!
  * **Easy Backup** - A basic backup feature that allows you to backup or restore server files with just one-click.
  * **Easy Action** - No command knowledge is required. Start/Stop/Restart your server with just a click.
  * **Discord Alert** - Get alerted when your servers encounters any special issues
  * **Auto Restart** - A basic function to auto restart your server if it crashes. Therefore, your server will always be up!
  * **Auto Update** - Checks for updates every 30 minutes, making sure your game servers are up to date!
  * **Restart Crontab** - Schedule your game servers to restart at any time, very useful for servers that require restarts to run smoothly.
  * **Start on Login** - Auto start WindowsGSM and game servers on user login.
  * **CPU Priority and Affinity** - Set and save the cpu priority and affinity!
  * **Mod Manager** - Manage your server custom mods with WindowsGSM!
  * **Remote Control** - Remote control your game servers with Discord!
  * **Plugins** - Allow support more game servers by installing plugins

### Plugins
Allow support more game servers by installing plugins!
##### Some available plugins (Made by [BattlefieldDuck](https://github.com/BattlefieldDuck) and [GTVolk](https://github.com/GTVolk)):
1. [WindowsGSM.ARMA2](https://github.com/DevVault/WindowsGSM.ARMA2)
2. [WindowsGSM.ARMA2OA](https://github.com/DevVault/WindowsGSM.ARMA2)
3. [WindowsGSM.ARMA3](https://github.com/BattlefieldDuck/WindowsGSM.ARMA3)
3. [WindowsGSM.HLDM](https://github.com/DevVault/WindowsGSM.HLDM)
3. [WindowsGSM.L4D](https://github.com/DevVault/WindowsGSM.L4D)
4. [WindowsGSM.PaperMC](https://github.com/BattlefieldDuck/WindowsGSM.PaperMC)

![Screenshot Plugins](https://windowsgsm.com/assets/images/WindowsGSM-Plugins-v1.21.0.png)

## Documentation
  * Website: <https://docs.windowsgsm.com/>
  * Github: <https://github.com/WindowsGSM/Documentation>

## Community
  * MeFriendos: <https://mefriendos.de/>
  * Discord: <https://discord.gg/vNJPTWY3UW>
  * Upstream community: [WindowsGSM Discord](https://discord.gg/bGc7t2R)
  * Original WindowsGSM Steam group: <https://steamcommunity.com/groups/WindowsGSM>

## Authors & Credits
  * **TatLead** - *Initial work* - [BattlefieldDuck](https://github.com/BattlefieldDuck)
  * **Deathreus** - *Code optimization* - [Deathreus](https://github.com/Deathreus)
  * **! AssaultLine** - *Mordhau server test*
  * **Dennis** - *Mordhau server test*
  * **Raziel** - *Various small improvements and external Plugins*

### Give Love!
If you like to thank me for the help over at the discord or the small changes in this fork, feel free to give me a tip:

[Buy me a coffee](https://ko-fi.com/raziel7893)

[Paypal](https://paypal.me/raziel7893)

For the Programm in general have a look over at its patreon:

[WGSM_Patreon](https://www.patreon.com/join/WindowsGSM)



### License
This project is licensed under the MIT License - see [LICENSE](LICENSE) for details.

## Stargazers over time
[![Stargazers over time](https://starchart.cc/PapaGordon/WindowsGSM.svg)](https://starchart.cc/PapaGordon/WindowsGSM)
