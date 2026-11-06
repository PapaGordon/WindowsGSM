# Development and releases

## Branches

Use `main` for development and releases in `PapaGordon/WindowsGSM`. The `origin` remote points to our fork, and `upstream` points to Raziel7893's fork.

To bring in changes from Raziel's `master` branch, run these commands from the repository root.

```powershell
git switch main
git fetch upstream
git merge upstream/master
```

Resolve any conflicts and check our version numbers and release URLs before building. To send selected changes to Raziel, start a separate contribution branch from `upstream/master` and apply only the commits that belong in the pull request.

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

Extract the ZIP into an empty folder and check the packaged EXE, docs and Steam helper. Run the checks for the server modules changed in that release. Use a new tag on `main` and publish the artifacts on [PapaGordon/WindowsGSM Releases](https://github.com/PapaGordon/WindowsGSM/releases).

Keep project and contributor credits in [CREDITS.md](../../CREDITS.md). Preserve the original copyright and artwork notices in the imported files and [licenses](../../licenses/).
