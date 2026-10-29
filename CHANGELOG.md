# Changelog

## 1.25.4.2 - 2026-10-29

- Supports optional Windows user-bound DPAPI encryption for the saved Steam password, with a setup script.
- Adds Minecraft: NeoForge beside ATM10, using official installers, version selection within a chosen Minecraft family and per-server firewall rules.
- Gives NeoForge its own official fox icon, with the original artwork credits and license.
- Stages NeoForge updates with a full backup while preserving worlds, mods, configuration and JVM arguments.

- Removes unconditional SteamCMD firewall allowances from installation and version checks; all server rules follow Auto Firewall.
- Downloads the Vanilla JAR without requiring Java first and preserves the original installation error.
- Reworks the built-in Vanilla Minecraft server and keeps the existing Java Edition server name.
- Validates Mojang downloads before replacing the runtime, keeps previous runtime files and recovers failed activations.
- Reads versions directly from the server JAR and supports exact stable release pins.
- Detects each release's required 64-bit Java version and preserves existing RAM parameters and server settings.
- Uses central per-server TCP game and UDP query rules for Vanilla instead of adding a global Java firewall exception.
- Recognizes the standalone Minecraft_Java plugin identifier and adds Vanilla filesystem/compiler/firewall checks.

## 1.25.4.1 - 2026-10-24

- Adds embedded DayZ console output from the current RPT file in the configured profile folder.
- Follows new log lines, rotation and shutdown output without replaying older sessions.
- Fixes native DayZ window detection and keeps Toggle Console working when the window starts hidden.
- Stops waiting indefinitely for a native window and keeps the saved visibility setting.
- Disables command entry for the DayZ log view.

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
