# Development

## Branches and remotes

- `main` is our development and release branch in `PapaGordon/WindowsGSM`.
- `master` keeps the existing upstream pull request at its current commit. Do not use it for our regular development.
- `origin` points to our repository and is the normal push target.
- `upstream` points to `Raziel7893/WindowsGSM`. Fetching works; direct pushes are disabled in this checkout.

In GitHub Desktop, use `main` as the current branch and **Push origin** to upload your changes. A push does not create a pull request. A request is only created when you choose to open one.

## Bring in upstream changes

```powershell
git switch main
git fetch upstream
git merge upstream/master
```

Resolve any conflicts, review the result and build it before pushing `main` to `origin`. Keep our version numbers and release URLs when resolving an upstream version change. This merge only updates our branch.

## Send selected changes upstream

Create a separate contribution branch from `upstream/master`, apply only the commits you want to share and push that branch to `origin`. Then open a pull request against `Raziel7893/WindowsGSM`. Keep unrelated MeFriendos changes on `main`.

## Build a release

Use Visual Studio MSBuild with the .NET 8 SDK and Windows desktop build tools. Update both version attributes in `WindowsGSM/Properties/AssemblyInfo.cs` and the project version fields before building.

```powershell
msbuild WindowsGSM/WindowsGSM.csproj /restore /t:Publish /p:Configuration=Release /p:RuntimeIdentifier=win-x64 /p:SelfContained=true /p:PublishSingleFile=true /p:PublishReadyToRun=false /p:PublishDir=../publish/MeFriendos/
```

Create a new version tag on `main` and publish the executable on `PapaGordon/WindowsGSM`. Do not move an existing version tag.

Include `LICENSE`, `CREDITS.md` and the `licenses` directory in release packages. Keep project and contributor credits in [CREDITS.md](CREDITS.md) instead of adding author bylines to user guides. Preserve copyright and license notices in imported code and artwork files.
