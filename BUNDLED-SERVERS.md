# Built-in MeFriendos servers

These servers are compiled into WindowsGSM. No files in the plugins folder are needed to install, import or run them. Their standalone repositories remain separate and usable on compatible WindowsGSM versions, including builds that do not use this fork.

The sources in WindowsGSM/GameServer/Bundled are snapshots of the repositories below. Only their namespace changes to WindowsGSM.GameServer.Bundled. Keep both copies in sync when changing server behavior. The original license notices are in licenses/WindowsGSM.*.txt and are embedded in the compiled assembly.

| Server | Standalone repository | Imported revision |
| --- | --- | --- |
| Arma 3 Dedicated Server | [WindowsGSM.ARMA3-MeFriendos](https://github.com/PapaGordon/WindowsGSM.ARMA3-MeFriendos) | `76f58baade48a349328459d5b65420a3294ec9df` |
| Minecraft: All the Mods 10 (ATM10) | [WindowsGSM.ATM10-MeFriendos](https://github.com/PapaGordon/WindowsGSM.ATM10-MeFriendos) | `e372350094ee2b09c56fa8ee2fe2f8213278be91` |
| Enshrouded Dedicated Server | [WindowsGSM.Enshrouded](https://github.com/PapaGordon/WindowsGSM.Enshrouded) | `57e7d5c5a783a4973f767a1c0d2f6c4adeea8c4f` |
| Farming Simulator 22 Dedicated Server | [WindowsGSM.FarmingSimulator22](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator22) | `ee7459d100f46d9491079a5f15cd720e344f6a5d` |
| Farming Simulator 25 Dedicated Server | [WindowsGSM.FarmingSimulator25](https://github.com/PapaGordon/WindowsGSM.FarmingSimulator25) | `3e988c7742180266b7691b699c7d1c294b6d6c67` |
| Project Zomboid Dedicated Server | [WindowsGSM.ProjectZomboid-MeFriendos](https://github.com/PapaGordon/WindowsGSM.ProjectZomboid-MeFriendos) | `7915c73255dbd8edcb84faa57ba5dc754d24b9fc` |
| SCP: Secret Laboratory Dedicated Server | [WindowsGSM.SCPSL-MeFriendos](https://github.com/PapaGordon/WindowsGSM.SCPSL-MeFriendos) | `8a207683cc3f363caab101c7db150e16112ac159` |
| Valheim Dedicated Server | [WindowsGSM.Valheim-MeFriendos](https://github.com/PapaGordon/WindowsGSM.Valheim-MeFriendos) | `3536aa02111167377f8864eb050cee400df0978f` |

Existing servergame values such as `Enshrouded Dedicated Server [Enshrouded.cs]` resolve to the built-in implementation. WindowsGSM does not rewrite the configuration. Installed copies remain visible in plugin management, but duplicate choices are hidden in Install and Import. Other third-party plugins still load as before.

ATM10 still needs a server pack from CurseForge. Place it in the WindowsGSM root or the serverfiles folder. Farming Simulator still needs the matching game license and dedicated-server setup. Valheim still requires changing its placeholder password. These requirements are the same as in the standalone plugins.

Auto Firewall also covers the game executable started by the Farming Simulator managers and SCP:SL LocalAdmin. ATM10 uses the configured TCP game port and UDP query port, with separate rule names for each server. It does not add a global exception for a shared Java installation.
