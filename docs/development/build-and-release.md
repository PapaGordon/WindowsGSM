# Building and releasing

We develop and release from `main` in `PapaGordon/WindowsGSM`. We no longer merge Raziel7893's fork as a whole, but we can still pick single fixes from it. Earlier imports are listed in [changes inherited from Raziel7893](upstream-changes.md).

## What you need

- Visual Studio with the .NET desktop workload, so you have the x64 MSBuild and the .NET 8 SDK. `dotnet build` alone doesn't work, because the project uses a COM reference for the Windows firewall.
- The .NET 10 SDK, only for the Minecraft tests.

## Build

Run these from the repository root in a Visual Studio Developer PowerShell. The first line builds everything, the second creates the single `WindowsGSM.exe` with the .NET runtime included.

```powershell
msbuild WindowsGSM.sln /restore /p:Configuration=Release
msbuild WindowsGSM/WindowsGSM.csproj /restore /t:Publish /p:Configuration=Release /p:RuntimeIdentifier=win-x64 /p:SelfContained=true /p:PublishSingleFile=true /p:PublishReadyToRun=false /p:PublishDir=../publish/MeFriendos/
```

GitHub Actions builds the solution on every push to `main` that touches the code.

## Tests

The tests are small console programs under `tests/`. Most of them need the Release build from above. Build a test with `msbuild tests/<folder>/<project>.csproj /restore /p:Configuration=Release` and run the EXE from its `bin/Release/<framework>/` folder. They report each check as `PASS` and end with an error code when one fails. None of them start a real game server, sign in to Steam or change firewall rules.

| Folder | What it covers |
| --- | --- |
| `Workshop` | DayZ and Arma 3 Workshop updates: mod list, unchanged mods, backups, rollback, start with installed mods |
| `BuiltInServers` | Registration, configs and start parameters of the built-in servers. Pass the standalone `DayZ.cs` path to compile the plugin too. |
| `DayZConsole` | DayZ log output in the embedded console. Needs the standalone `DayZ.cs` path as argument. |
| `SteamCredentials` | Encrypted Steam passwords and the helper script |
| `SourceConsole` | Embedded console for Source servers |
| `Minecraft` | Vanilla Minecraft versions, downloads and updates. Run with `dotnet run --project tests/Minecraft/MinecraftChecks.csproj` (.NET 10 SDK). |
| `NeoForge` | NeoForge versions, Java selection, import and updates |
| `ATM10` | ATM10 pack updates |
| `PluginCompatibility` | Compiles our standalone plugins against the WindowsGSM plugin API |
| `PublishSmokeCheck` | Checks the published `WindowsGSM.exe` itself |

Some folders have their own README with details. Before a release, run at least the tests for every server you changed.

## Release

1. Raise the version in `WindowsGSM/Properties/AssemblyInfo.cs` (`AssemblyVersion` and `AssemblyFileVersion`) and in `WindowsGSM/WindowsGSM.csproj` (`Version` and `ApplicationVersion`).
2. Add the changes to `CHANGELOG.md` and write the release text in `releases/v<version>.md`.
3. Build and publish as shown above.
4. Put these files into `WindowsGSM-v<version>-win-x64.zip`, keeping their folders:
   - the published `WindowsGSM.exe`, `README.md` and `CHANGELOG.md`
   - `docs/`
   - `Encrypt-SteamPassword.cmd` and `tools/Set-SteamCredentials.ps1`
   - `LICENSE`, `CREDITS.md` and `licenses/`
5. Extract the ZIP into an empty folder and check the EXE, the docs and the Steam helper.
6. Create `WindowsGSM-v<version>-SHA256SUMS.txt` with one line per file: the SHA-256 hash in lower case, two spaces, the file name.
7. Commit, tag `v<version>` on `main`, push, and publish a release named `WindowsGSM v<version>` on [GitHub](https://github.com/PapaGordon/WindowsGSM/releases) with the EXE, the ZIP and the checksum file.

Use `/` as the folder separator inside the ZIP. `Compress-Archive` in Windows PowerShell 5.1 writes `\` instead, which some programs unpack as part of the file name.

## Credits and licenses

Keep project and contributor credits in [CREDITS.md](../../CREDITS.md). Leave the original copyright and artwork notices in imported files and in [licenses](../../licenses/) untouched.
