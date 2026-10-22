# Changelog

## 1.25.3.0 - 2026-10-22

First MeFriendos release from our own development branch.

- Includes the per-server Auto Firewall switch with saved settings and synchronization before startup.
- Keeps manual port rules and other servers' automatic rules unchanged.
- Makes the shared process-stop helper accessible to plugins, fixing the solution build for the bundled Valheim example.
- Uses our repository for software update checks and issue reports.
- Shows the MeFriendos build name and version in the window title.
- Ships a Windows x64 executable with the .NET runtime included.

Based on Raziel7893/WindowsGSM v1.25.2.2. See [ChangesFromBase.md](ChangesFromBase.md) for the inherited changes from the original WindowsGSM project.
