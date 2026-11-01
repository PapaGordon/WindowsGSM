# WindowsGSM - MeFriendos credits

This is the **MeFriendos fork of WindowsGSM**, owned and maintained by [PapaGordon](https://github.com/PapaGordon) for [MeFriendos](https://mefriendos.de).

PapaGordon maintains the MeFriendos changes, server integrations and standalone plugins. The original WindowsGSM code, upstream improvements, imported plugin code and artwork keep their own authors and copyright notices.

## WindowsGSM and upstream work

| Project or contribution | Credit |
| --- | --- |
| Original WindowsGSM | **TatLead / [BattlefieldDuck](https://github.com/BattlefieldDuck)** and the [WindowsGSM contributors](https://github.com/WindowsGSM/WindowsGSM/graphs/contributors). The original copyright notice is retained in [LICENSE](LICENSE). |
| Parent fork used by MeFriendos | **[Raziel7893](https://github.com/Raziel7893)** and the contributors to [Raziel7893/WindowsGSM](https://github.com/Raziel7893/WindowsGSM). This includes the .NET 8 base and the backup, crontab and webhook improvements described in [ChangesFromBase.md](ChangesFromBase.md). |
| Code optimization credited by the original project | **[Deathreus](https://github.com/Deathreus)**. |
| Mordhau server testing credited by the original project | **! AssaultLine** and **Dennis**. |

The inherited Git history also records contributions under these names: **Alex, Alexander Barthel, BizakDaTroll, DiFFoZ, Ethan Snow, Eugene, Felix, Kexodus, Kyle, Kyle Carlson, Max Vorobyev, Scott Walters, SettingDust, Soul, Stian T and Vladislav**. Names follow the commit metadata; some contributors used more than one name.

The [commit history](https://github.com/PapaGordon/WindowsGSM/commits/main/) and [contributor list](https://github.com/PapaGordon/WindowsGSM/graphs/contributors) record contributions to this fork and its inherited code. Thanks also to the community members who report bugs, share logs and test changes on their servers.

## Built-in modules and plugin sources

The MeFriendos versions are maintained by PapaGordon. Imported code is credited below; its original notices remain in [licenses](licenses/).

| Module | Source and credit |
| --- | --- |
| Arma 3 | [BattlefieldDuck/WindowsGSM.ARMA3](https://github.com/BattlefieldDuck/WindowsGSM.ARMA3), copyright TatLead. [MildlyInterested's fork](https://github.com/MildlyInterested/WindowsGSM.ARMA3) was also used as a 64-bit reference. See [license](licenses/WindowsGSM.ARMA3.txt). |
| Enshrouded | **ohmcodes**, [original plugin](https://github.com/ohmcodes/WindowsGSM.Enshrouded). See [license](licenses/WindowsGSM.Enshrouded.txt). |
| Project Zomboid | **Richard Beard / DoctorBeardz**, [original plugin](https://github.com/DoctorBeardz/WindowsGSM.ProjectZomboid). See [license](licenses/WindowsGSM.ProjectZomboid.txt). |
| SCP: Secret Laboratory | **Oscar Hurst**, [original plugin](https://github.com/Voidoz/WindowsGSM.SCPSL). See [license](licenses/WindowsGSM.SCPSL.txt). |
| Valheim | **Sarpendon**, [original plugin](https://github.com/Sarpendon/WindowsGSM.Valheim). See [license](licenses/WindowsGSM.Valheim.txt). |
| ATM10 | **PapaGordon / MeFriendos**, [WindowsGSM.ATM10](https://github.com/PapaGordon/WindowsGSM.ATM10). The modpack itself is by the **All the Mods Team** and its mod authors. See [plugin license](licenses/WindowsGSM.ATM10.txt). |
| Farming Simulator 22 and 25 | **PapaGordon / MeFriendos**, [FS22 plugin](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator22) and [FS25 plugin](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator25). See [FS22 license](licenses/WindowsGSM.FarmingSimulator22.txt) and [FS25 license](licenses/WindowsGSM.FarmingSimulator25.txt). |
| Minecraft NeoForge | **PapaGordon / MeFriendos**, [WindowsGSM.Minecraft_NeoForge](https://github.com/PapaGordon/WindowsGSM.Minecraft_NeoForge). The NeoForge loader and installer are by the **NeoForged team and contributors**. |
| DayZ and Vanilla Minecraft | Original WindowsGSM server support, with the current MeFriendos implementations and integration maintained by **PapaGordon**. See [DayZ](https://github.com/PapaGordon/WindowsGSM.DayZ) and the [built-in Vanilla module](MINECRAFT.md). |

Import revisions and links to the maintained plugins are recorded in [BUNDLED-SERVERS.md](BUNDLED-SERVERS.md). Other server modules and plugin examples inherited from WindowsGSM remain credited through their source and upstream history.

## Artwork and game assets

Game names, logos and icons belong to their respective creators and rights holders. They identify the supported games and are not covered by the code's MIT license.

| Artwork | Credit |
| --- | --- |
| NeoForge fox | **Ridanisaurus**, copyright the **NeoForged team**, under **CC BY 4.0**. The ICO is included unchanged. See [attribution and source](licenses/NeoForge-art-attribution.txt) and the [license text](licenses/NeoForge-art-CC-BY-4.0.txt). |
| ATM10 logo | **All the Mods Team**. See [source and artwork notes](licenses/Native-game-artwork.txt). |
| Project Zomboid / Spiffo | **The Indie Stone**. The original Steam artwork was converted to PNG. See [artwork notes](licenses/Native-game-artwork.txt). |
| SCP: Secret Laboratory | **Northwood Studios and the respective artwork rights holders**. The original Steam artwork was converted to PNG. See [artwork notes](licenses/Native-game-artwork.txt). |
| Arma 3 and DayZ | **Bohemia Interactive** and the respective artwork creators. |
| Enshrouded | **Keen Games** and the respective artwork creators. |
| Farming Simulator | **GIANTS Software** and the respective artwork creators. |
| Minecraft | **Mojang Studios / Microsoft** and the respective artwork creators. |
| Valheim | **Iron Gate Studio** and the respective artwork rights holders. |

The remaining game icons inherited from WindowsGSM keep the rights of their original creators. No affiliation or endorsement by a game developer, publisher or upstream project is implied.

## Libraries and tools

Thanks to the authors and maintainers of the libraries used by WindowsGSM:

- **Microsoft and the .NET contributors** for .NET, WPF, Windows Forms, Roslyn and the Microsoft/System libraries used by the project.
- **CoreRCON** for RCON support.
- **Discord.Net** for the Discord integration.
- **MahApps.Metro and ControlzEx** for the desktop interface.
- **LiveCharts** for charts.
- **NCrontab** for cron schedules.
- **SharpZipLib** for archive support.
- **Newtonsoft.Json** for JSON handling.

Package references are recorded in [WindowsGSM.csproj](WindowsGSM/WindowsGSM.csproj). Package authors and license details are also recorded in the packages themselves, including their dependencies. These libraries keep their own licenses and notices.

Valve provides **SteamCMD and Steam Workshop**. Mojang provides the **Minecraft server software**, and the NeoForged team provides the **NeoForge installer**. The games, modpacks and individual mods remain the work of their own developers and contributors.

## License and attribution

The project uses the [MIT License](LICENSE). Its original **Copyright (c) 2019 TatLead** notice is retained. A separate notice identifies the MeFriendos modifications by **Brandon Hanke (PapaGordon / MeFriendos)**.

This credits file is the central project overview. It does not replace the copyright, permission or artwork notices in [LICENSE](LICENSE), [licenses](licenses/) or imported files. Keep those notices with redistributed copies.
