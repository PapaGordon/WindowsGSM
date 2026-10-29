# Workshop updater checks

Made by PapaGordon

Build WGSM Release, then run `dotnet run --project tests/Workshop/WorkshopChecks.csproj -c Release`.

The suite uses synthetic credentials and a fake download runner. It checks opt-in parsing, argument merging, complete-folder replacement, preservation of unlisted mods and manual keys, key rotation, rollback after an actual Windows file lock, recovery markers and individual SteamCMD success receipts. No Steam login or game server is started. Test credentials are created only under this test's bin folder; existing SteamCMD data there is never overwritten.
