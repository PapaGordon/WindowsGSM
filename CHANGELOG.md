# Changelog

## 1.25.4.0 - 2026-10-24

- Builds in Arma 3, ATM10, Enshrouded, Farming Simulator 22/25, Project Zomboid, SCP: Secret Laboratory and Valheim, including their icons.
- Keeps standalone plugin repositories available and recognizes existing plugin server names without requiring the plugin files.
- Hides duplicate entries for the bundled plugins in Install and Import.
- Fixes DayZ name, slot and query-port synchronization, mission selection, custom config paths and duplicate mod arguments.
- Creates DayZ defaults locally, keeps unrelated settings and backs up changed configurations.
- Resolves the DayZ executable before central Auto Firewall runs and tries a shutdown signal before forced termination.
- Uses this fork for built-in game icons in Discord webhooks.
- Covers the Farming Simulator and SCP:SL game processes in Auto Firewall, and uses separate game/query port rules for each ATM10 server.
- Returns a normal SteamCMD error when an update cannot start a process.

## 1.25.3.0 - 2026-10-22

First MeFriendos release from our own development branch.

- Includes the per-server Auto Firewall switch with saved settings and synchronization before startup.
- Keeps manual port rules and other servers' automatic rules unchanged.
- Makes the shared process-stop helper accessible to plugins, fixing the solution build for the bundled Valheim example.
- Uses our repository for software update checks and issue reports.
- Shows the MeFriendos build name and version in the window title.
- Ships a Windows x64 executable with the .NET runtime included.

Based on Raziel7893/WindowsGSM v1.25.2.2. See [ChangesFromBase.md](ChangesFromBase.md) for the inherited changes from the original WindowsGSM project.
