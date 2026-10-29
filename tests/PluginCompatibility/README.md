# Plugin and encrypted login checks

Build WindowsGSM in Release first. Run `dotnet run --project tests/PluginCompatibility/PluginChecks.csproj -- <manifest.json> <standalone-neoforge.cs>`.

The manifest is an array of objects with `Repo` and `Source` (an array containing the absolute main plugin source path). Pass the current Arma 3, ATM10, DayZ, Enshrouded, Farming Simulator 22/25, Project Zomboid, SCP:SL and Valheim sources. The deleted standalone Vanilla repository is not required.

The suite compiles each source with the real WGSM plugin API, checks both account-login parameter paths with synthetic DPAPI credentials, and rejects damaged encrypted data before SteamCMD starts. It also checks anonymous downloads, the built-in server counterparts, both NeoForge identifiers and the DayZAuto SDK example. The SDK path is resolved relative to a manifest in the repository's publish folder.

All credential files are created under the test executable's own bin folder. A deliberately invalid SteamCMD fixture prevents accidental real logins. Existing SteamCMD files are never overwritten. No game server is started.
