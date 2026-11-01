# WindowsGSM - MeFriendos

Our WindowsGSM build, based on [Raziel7893/WindowsGSM](https://github.com/Raziel7893/WindowsGSM).

[Download the latest release](https://github.com/PapaGordon/WindowsGSM/releases/latest) | [Changelog](CHANGELOG.md) | [Credits](CREDITS.md) | [Development](DEVELOPMENT.md)

The Windows x64 release includes the .NET runtime. Stop WindowsGSM before replacing `WindowsGSM.exe`. Keep your existing server, plugin and configuration folders.

This build includes a per-server **Auto Firewall** switch. Its state is saved for each server and applied when you change the switch and before startup. Turn it off if you manage the server's port rules yourself.

Arma 3, ATM10, Enshrouded, Farming Simulator 22/25, Project Zomboid, SCP: Secret Laboratory and Valheim are built in. They appear in Install and Import without an extra plugin download. The [standalone plugins](BUNDLED-SERVERS.md) remain available for other WindowsGSM builds. Existing server configurations with our old plugin names still work.

DayZ uses the existing built-in entry, with synchronized name, slots and query port, mission selection and local default configuration. See [DayZ setup](DAYZ.md). DayZ and Arma 3 can also [update configured Workshop mods before starting](WORKSHOP.md).

**Minecraft: Java Edition Server** now uses our [Vanilla module](MINECRAFT.md). **Minecraft: NeoForge** is available as a separate entry without a modpack; [ATM10 stays separate](NEOFORGE.md). A [standalone NeoForge plugin](https://github.com/PapaGordon/WindowsGSM.Minecraft_NeoForge) is also available for other compatible WGSM hosts.

You can save the Steam password encrypted for your Windows user. The release ZIP includes `Encrypt-SteamPassword.cmd` and the setup script. Follow the [password setup steps](STEAM-CREDENTIALS.md) after updating WindowsGSM.
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
| <img src="WindowsGSM/Images/Games/arma3.png?raw=true" alt="" height="24" /> | Arma 3 Dedicated Server |✅| A2S |
| <img src="WindowsGSM/Images/Games/atm10.png?raw=true" alt="" height="24" /> | Minecraft: All the Mods 10 (ATM10) |✅| UT3 |
| <img src="WindowsGSM/Images/Games/enshrouded.png?raw=true" alt="" height="24" /> | Enshrouded Dedicated Server |✅| A2S |
| <img src="WindowsGSM/Images/Games/fs22.png?raw=true" alt="" height="24" /> | Farming Simulator 22 Dedicated Server |✅| |
| <img src="WindowsGSM/Images/Games/fs25.png?raw=true" alt="" height="24" /> | Farming Simulator 25 Dedicated Server |✅| |
| <img src="WindowsGSM/Images/Games/pz.png?raw=true" alt="" height="24" /> | Project Zomboid Dedicated Server |✅| A2S |
| <img src="WindowsGSM/Images/Games/scpsl.png?raw=true" alt="" height="24" /> | SCP: Secret Laboratory Dedicated Server |✅| |
| <img src="WindowsGSM/Images/Games/valheim.png?raw=true" alt="" height="24" /> | Valheim Dedicated Server |✅| A2S |
| <img src="WindowsGSM/Images/Games/7dtd.png?raw=true" alt="" height="24" />   | 7 Days to Die Dedicated Server                    |✅|      |
| <img src="WindowsGSM/Images/Games/arkse.png?raw=true" alt="" height="24" />  | ARK: Survival Evolved Dedicated Server            |✅| A2S  |
| <img src="WindowsGSM/Images/Games/avorion.png?raw=true" alt="" height="24" />| Avorion Dedicated Server                          |✅|      |
| <img src="WindowsGSM/Images/Games/bt.png?raw=true" alt="" height="24" />     | Barotrauma Dedicated Server                       |✅|      |
| <img src="WindowsGSM/Images/Games/bw.png?raw=true" alt="" height="24" />     | BlackWake Dedicated Server                        |✅|      |
| <img src="WindowsGSM/Images/Games/ce.png?raw=true" alt="" height="24" />     | Conan Exiles Dedicated Server                     |✅| A2S  |
| <img src="WindowsGSM/Images/Games/cs.png?raw=true" alt="" height="24" />     | Counter-Strike: 1.6 Dedicated Server              |✅| A2S  |
| <img src="WindowsGSM/Images/Games/cscz.png?raw=true" alt="" height="24" />   | Counter-Strike: Condition Zero Dedicated Server   |✅| A2S  |
| <img src="WindowsGSM/Images/Games/csgo.png?raw=true" alt="" height="24" />   | Counter-Strike: Global Offensive Dedicated Server |✅| A2S  |
| <img src="WindowsGSM/Images/Games/css.png?raw=true" alt="" height="24" />    | Counter-Strike: Source Dedicated Server           |✅| A2S  |
| <img src="WindowsGSM/Images/Games/dod.png?raw=true" alt="" height="24" />    | Day of Defeat Dedicated Server                    |✅| A2S  |
| <img src="WindowsGSM/Images/Games/dods.png?raw=true" alt="" height="24" />   | Day of Defeat: Source Dedicated Server            |✅| A2S  |
| <img src="WindowsGSM/Images/Games/dayz.png?raw=true" alt="" height="24" />   | DayZ Dedicated Server                             |✅| A2S  |
| <img src="WindowsGSM/Images/Games/dmc.png?raw=true" alt="" height="24" />    | Deathmatch Classic Dedicated Server               |✅| A2S  |
| <img src="WindowsGSM/Images/Games/eco.png?raw=true" alt="" height="24" />    | Eco Dedicated Server                              |✅|      |
| <img src="WindowsGSM/Images/Games/egs.png?raw=true" alt="" height="24" />    | Empyrion - Galactic Survival Dedicated Server     |✅|      |
| <img src="WindowsGSM/Images/Games/gmod.png?raw=true" alt="" height="24" />   | Garry's Mod Dedicated Server                      |✅| A2S  |
| <img src="WindowsGSM/Images/Games/gta5.png?raw=true" alt="" height="24" />   | Grand Theft Auto V Dedicated Server ([FiveM](https://github.com/citizenfx/fivem))|✅| FIVEM |
| <img src="WindowsGSM/Images/Games/hl2dm.png?raw=true" alt="" height="24" />  | Half-Life 2: Deathmatch Dedicated Server          |✅| A2S  |
| <img src="WindowsGSM/Images/Games/hlof.png?raw=true" alt="" height="24" />   | Half-Life: Opposing Force Dedicated Server        |✅| A2S  |
| <img src="WindowsGSM/Images/Games/heat.png?raw=true" alt="" height="24" />   | Heat Dedicated Server                             |✅|      |
| <img src="WindowsGSM/Images/Games/ins.png?raw=true" alt="" height="24" />    | Insurgency Dedicated Server                       |✅| A2S  |
| <img src="WindowsGSM/Images/Games/inss.png?raw=true" alt="" height="24" />   | Insurgency: Sandstorm Dedicated Server            |✅| A2S  |
| <img src="WindowsGSM/Images/Games/l4d2.png?raw=true" alt="" height="24" />   | Left 4 Dead 2 Dedicated Server                    |✅| A2S  |
| <img src="WindowsGSM/Images/Games/mcbe.png?raw=true" alt="" height="24" />   | Minecraft: Bedrock Edition Server                 |✅|      |
| <img src="WindowsGSM/Images/Games/mc.png?raw=true" alt="" height="24" />     | Minecraft: Java Edition Server                    |✅| UT3  |
| <img src="WindowsGSM/Images/Games/mcpe.png?raw=true" alt="" height="24" />   | Minecraft: Pocket Edition Server ([PocketMine-MP](https://github.com/pmmp/PocketMine-MP)) |✅| UT3  |
| <img src="WindowsGSM/Images/Games/mordhau.png?raw=true" alt="" height="24" />| Mordhau Dedicated Server                          |✅| A2S  |
| <img src="WindowsGSM/Images/Games/nmrih.png?raw=true" alt="" height="24" />  | No More Room in Hell Dedicated Server             |✅| A2S  |
| <img src="WindowsGSM/Images/Games/onset.png?raw=true" alt="" height="24" />  | Onset Dedicated Server                            |✅|      |
| <img src="WindowsGSM/Images/Games/olow.png?raw=true" alt="" height="24" />   | Outlaws of the Old West Dedicated Server          |✅| A2S  |
| <img src="WindowsGSM/Images/Games/ps.png?raw=true" alt="" height="24" />     | Post Scriptum Dedicated Server                    |✅| A2S  |
| <img src="WindowsGSM/Images/Games/rok.png?raw=true" alt="" height="24" />    | Reign Of Kings Dedicated Server                   |✅|      |
| <img src="WindowsGSM/Images/Games/rcc.png?raw=true" alt="" height="24" />    | Ricochet Dedicated Server                         |✅| A2S  |
| <img src="WindowsGSM/Images/Games/rcc.png?raw=true" alt="" height="24" />    | Risk of Rain 2 Dedicated Server                   |✅| A2S  |
| <img src="WindowsGSM/Images/Games/rust.png?raw=true" alt="" height="24" />   | Rust Dedicated Server                             |✅| A2S  |
| <img src="WindowsGSM/Images/Games/se.png?raw=true" alt="" height="24" />     | Space Engineers Dedicated Server                  |✅| A2S  |
| <img src="WindowsGSM/Images/Games/sq.png?raw=true" alt="" height="24" />     | Squad Dedicated Server                            |✅| A2S  |
| <img src="WindowsGSM/Images/Games/sw.png?raw=true" alt="" height="24" />     | Stormworks Dedicated Server                       |✅|      |
| <img src="WindowsGSM/Images/Games/tf2.png?raw=true" alt="" height="24" />    | Team Fortress 2 Dedicated Server                  |✅| A2S  |
| <img src="WindowsGSM/Images/Games/tfc.png?raw=true" alt="" height="24" />    | Team Fortress Classic Dedicated Server            |✅| A2S  |
| <img src="WindowsGSM/Images/Games/tf.png?raw=true" alt="" height="24" />     | The Forest Dedicated Server                       |✅|      |
| <img src="WindowsGSM/Images/Games/unt.png?raw=true" alt="" height="24" />    | Unturned Dedicated Server                         |✅| A2S  |
| <img src="WindowsGSM/Images/Games/zps.png?raw=true" alt="" height="24" />    | Zombie Panic Source Dedicated Server              |✅| A2S  |

|               | 🎮 Game Server  | Plugin supported | Query |
| ------------- | --------------- | --------- | ----- |
| <img src="WindowsGSM/Images/Games/arma3.png?raw=true" alt="" height="24" /> | Arma 3 Dedicated Server |✅| A2S |
| <img src="WindowsGSM/Images/Games/atm10.png?raw=true" alt="" height="24" /> | Minecraft: All the Mods 10 (ATM10) |✅| UT3 |
| <img src="WindowsGSM/Images/Games/enshrouded.png?raw=true" alt="" height="24" /> | Enshrouded Dedicated Server |✅| A2S |
| <img src="WindowsGSM/Images/Games/fs22.png?raw=true" alt="" height="24" /> | Farming Simulator 22 Dedicated Server |✅| |
| <img src="WindowsGSM/Images/Games/fs25.png?raw=true" alt="" height="24" /> | Farming Simulator 25 Dedicated Server |✅| |
| <img src="WindowsGSM/Images/Games/pz.png?raw=true" alt="" height="24" /> | Project Zomboid Dedicated Server |✅| A2S |
| <img src="WindowsGSM/Images/Games/scpsl.png?raw=true" alt="" height="24" /> | SCP: Secret Laboratory Dedicated Server |✅| |
| <img src="WindowsGSM/Images/Games/valheim.png?raw=true" alt="" height="24" /> | Valheim Dedicated Server |✅| A2S |
| <img src="https://github.com/DevVault/WindowsGSM.ARMA2/blob/master/ARMA2.cs/ARMA2.png?raw=true" alt="" height="24" />   | Arma2   |✅| A2S     |
| <img src="https://github.com/DevVault/WindowsGSM.ARMA2/blob/master/ARMA2OA.cs/ARMA2OA.png?raw=true" alt="" height="24" />   | Arma2: OA   |✅| A2S     |
| <img src="https://github.com/1stian/imgrepo/blob/master/WindowsGSMrepo/arma3.png?raw=true" alt="" height="24" />   | Arma3   |✅|      |
| <img src="https://github.com/1stian/imgrepo/blob/master/WindowsGSMrepo/astroneer.png?raw=true" alt="" height="24" />   | ASTRONEER   |✅|      |
| <img src="https://github.com/1stian/imgrepo/blob/master/WindowsGSMrepo/deadmatter.png?raw=true" alt="" height="24" />   | Dead Matter   |✅| UT3     |
| <img src="https://github.com/1stian/imgrepo/blob/master/WindowsGSMrepo/papermc.png?raw=true" alt="" height="24" />   | PaperMC   |✅| A2S     |
| <img src="https://github.com/DevVault/WindowsGSM.HLDM/blob/master/HLDM.cs/HLDM.png?raw=true" alt="" height="24" />   | Half-Life: Deathmatch   |✅| A2S     |
| <img src="https://github.com/DevVault/WindowsGSM.L4D/blob/master/L4D.cs/L4D.png?raw=true" alt="" height="24" />   | Left 4 Dead   |✅| A2S     |
| <img src="https://github.com/1stian/imgrepo/blob/master/WindowsGSMrepo/waterfall.png?raw=true" alt="" height="24" />   | PaperMC: Waterfall   |✅| A2S     |
| <img src="https://github.com/1stian/imgrepo/blob/master/WindowsGSMrepo/spigot.png?raw=true" alt="" height="24" />   | Spigot   |✅| A2S     |

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
##### Community plugins by [BattlefieldDuck](https://github.com/BattlefieldDuck) and [GTVolk](https://github.com/GTVolk):
1. [WindowsGSM.ARMA2](https://github.com/DevVault/WindowsGSM.ARMA2)
2. [WindowsGSM.ARMA2OA](https://github.com/DevVault/WindowsGSM.ARMA2)
3. [WindowsGSM.ARMA3](https://github.com/BattlefieldDuck/WindowsGSM.ARMA3)
3. [WindowsGSM.HLDM](https://github.com/DevVault/WindowsGSM.HLDM)
3. [WindowsGSM.L4D](https://github.com/DevVault/WindowsGSM.L4D)
4. [WindowsGSM.PaperMC](https://github.com/BattlefieldDuck/WindowsGSM.PaperMC)

![Screenshot Plugins](https://windowsgsm.com/assets/images/WindowsGSM-Plugins-v1.21.0.png)

### Embedded Source console

Source servers, including Garry's Mod, keep their own Windows console when Embed Console is enabled. WGSM adds `-condebug` and shows new lines from the game's `console.log` or `console.txt`. This avoids the input-handle error that could crash Garry's Mod after loading. Commands still go to the native window.

Keep the default log filename for embedded output. Old entries stay in the file and are skipped on restart. You can archive the log while the server is stopped.

## Documentation
  * Website: <https://docs.windowsgsm.com/>
  * Github: <https://github.com/WindowsGSM/Documentation>

## Community
  * MeFriendos: <https://mefriendos.de/>
  * Discord: <https://discord.gg/vNJPTWY3UW>
  * Upstream community: [WindowsGSM Discord](https://discord.gg/bGc7t2R)
  * Original WindowsGSM Steam group: <https://steamcommunity.com/groups/WindowsGSM>

## Authors & Credits

The MeFriendos fork, original developers, plugin authors and artwork credits are listed in [CREDITS.md](CREDITS.md).

### Support upstream work

To support Raziel's work on the parent fork: [Ko-fi](https://ko-fi.com/raziel7893) or [PayPal](https://paypal.me/raziel7893).

To support the original WindowsGSM project: [WindowsGSM Patreon](https://www.patreon.com/join/WindowsGSM).

### License

This project uses the [MIT License](LICENSE), with the original copyright notice and a separate notice for the MeFriendos modifications. Imported code and artwork keep their own notices; see [CREDITS.md](CREDITS.md) and [licenses](licenses/).

## Stargazers over time
[![Stargazers over time](https://starchart.cc/PapaGordon/WindowsGSM.svg)](https://starchart.cc/PapaGordon/WindowsGSM)
