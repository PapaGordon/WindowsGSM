# Steam account and password

Some servers can only be downloaded with a Steam account, for example DayZ, Arma 3 and Farming Simulator. The same account downloads Workshop mods for DayZ and Arma 3. WindowsGSM keeps the login in `bin/steamcmd/userData.txt`. With our password helper, the password in that file is encrypted, so nobody can read it just by opening or copying the file.

## Set it up

1. Download the Windows ZIP from the [latest release](https://github.com/PapaGordon/WindowsGSM/releases/latest).
2. Close WindowsGSM and extract the ZIP into your WindowsGSM folder. Your `servers`, `configs`, `plugins` and `bin` folders stay as they are.
3. Sign in to Windows as the user that normally runs WindowsGSM. If WindowsGSM runs as a service or scheduled task under another account, run the helper as that account, with its user profile loaded.
4. Double-click **Encrypt-SteamPassword.cmd** in the WindowsGSM folder.
5. Enter your Steam login name, then your password. The password stays hidden while you type.
6. Wait for **Encrypted credentials saved for the current Windows user**.
7. The helper now signs in to Steam once. Enter your Steam Guard code if SteamCMD asks for it.
8. Wait for **Steam login saved**, close the window and start WindowsGSM.

If `userData.txt` doesn't exist yet, open the install screen in WindowsGSM, click **Set Account** once, close WindowsGSM and run the helper again.

Run the helper again when you change your Steam password, or when you move WindowsGSM to another machine or Windows user. The encrypted password only works for the Windows user on the machine that saved it.

## Steam Guard keeps asking

The password alone doesn't get past Steam Guard. What counts is the login that SteamCMD saves after you entered a Steam Guard code once. It lives in `bin\steamcmd\config\config.vdf`. Before every login, WindowsGSM tries this saved login first. When it works, the SteamCMD log shows `Logging in using cached credentials` and Steam doesn't ask for anything.

When the saved login is missing or has expired, WindowsGSM falls back to the password and Steam Guard asks again. Nobody is there to confirm it during an automatic restart. DayZ and Arma 3 then still start with the mods they already have, but the update is skipped.

If this happens:

- Run the helper again, or save the login by hand: close WindowsGSM, open a console in `bin\steamcmd`, run `steamcmd.exe`, type `login YourSteamLogin`, then your password and the Steam Guard code, and finish with `quit`.
- Keep enough free space on the drive. SteamCMD can lose its saved login when the drive runs full.

## Using PowerShell instead

This runs the helper for a WindowsGSM installation in `F:\WindowsGSM`. Change the path and login name to yours.

```powershell
Set-Location 'F:\WindowsGSM'
powershell.exe -NoProfile -ExecutionPolicy Bypass -File '.\tools\Set-SteamCredentials.ps1' -UserDataPath 'F:\WindowsGSM\bin\steamcmd\userData.txt' -SteamUser 'YourSteamLogin'
```

Type the password only at the hidden prompt, never into the command. Add `-SkipSteamLogin` if you only want to save the password without signing in to Steam.

The CMD file only starts this script. The execution policy setting applies to that one PowerShell window and doesn't change your system settings.

## What the helper changes

In `userData.txt`, the helper keeps your login name and comments, empties `steamPass` and adds `steamPassEncrypted` with the encrypted password. Don't copy that long value into `steamPass`. The helper never shows your password and doesn't keep a readable backup. Older copies of `userData.txt` from before may still contain the readable password, so delete those yourself.

Files without encryption still work. As soon as `steamPassEncrypted` exists, WindowsGSM only uses that and never falls back to the readable field, even if decryption fails.

## What it protects and what it doesn't

The encryption stops people who simply open or copy the file. It doesn't help against an administrator or a program running as your Windows user. When the saved Steam login doesn't work, WindowsGSM hands the password to SteamCMD on its command line, where other programs with enough rights can see it while SteamCMD runs. Don't share process lists or logs from that moment.

`config.vdf` lets anyone sign in as you without a password. Protect it like a password and never share it.

## Compatibility

You need a WindowsGSM build with support for encrypted passwords, such as ours from v1.25.4.2 on. Older builds and plugins that read `steamPass` themselves can't use the encrypted field. Our Arma 3, DayZ and Farming Simulator 22/25 plugins use the WindowsGSM login and work with it. Servers that download without an account don't need a saved password at all.

## For developers

`steamPassEncrypted` contains the UTF-8 password encrypted with Windows DPAPI (CurrentUser scope, no extra entropy), encoded as Base64. PowerShell's `ConvertFrom-SecureString` uses a different format, so write the field with the included helper.
