# Development and releases

We develop and release from `main` in `PapaGordon/WindowsGSM` and no longer merge changes from Raziel7893's fork. Earlier imports are listed in [upstream changes](upstream-changes.md).

## Build and publish locally

Use the x64 MSBuild that ships with Visual Studio, with the .NET 8 SDK and Windows desktop build tools installed. Before a new release, update `AssemblyVersion` and `AssemblyFileVersion` in `WindowsGSM/Properties/AssemblyInfo.cs`, and `Version` in `WindowsGSM/WindowsGSM.csproj`.

From the repository root in a Visual Studio Developer PowerShell, build the solution and publish the Windows x64 executable.

```powershell
msbuild WindowsGSM.sln /restore /p:Configuration=Release
msbuild WindowsGSM/WindowsGSM.csproj /restore /t:Publish /p:Configuration=Release /p:RuntimeIdentifier=win-x64 /p:SelfContained=true /p:PublishSingleFile=true /p:PublishReadyToRun=false /p:PublishDir=../publish/MeFriendos/
```

## Release ZIP

Keep these files and folders at their original paths inside the ZIP.

- The published `WindowsGSM.exe`, `README.md` and `CHANGELOG.md`.
- `docs/` with the setup and maintenance guides.
- `Encrypt-SteamPassword.cmd` and `tools/Set-SteamCredentials.ps1`.
- `LICENSE`, `CREDITS.md` and `licenses/`.

Name it `WindowsGSM-v<version>-win-x64.zip` and use `/` as the path separator inside it. `Compress-Archive` in Windows PowerShell 5.1 writes `\` instead, which some tools extract as file names.

Extract the ZIP into an empty folder and check the packaged EXE, docs and Steam helper. Run the checks for the server modules changed in that release. Use a new tag on `main` and publish the artifacts on [PapaGordon/WindowsGSM Releases](https://github.com/PapaGordon/WindowsGSM/releases): `WindowsGSM.exe`, the ZIP and `WindowsGSM-v<version>-SHA256SUMS.txt` with one `<sha256>  <file name>` line for each. The release text comes from `releases/v<version>.md`.

Keep project and contributor credits in [CREDITS.md](../../CREDITS.md). Preserve the original copyright and artwork notices in the imported files and [licenses](../../licenses/).
