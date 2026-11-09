# Save your Steam password encrypted

Starting with WindowsGSM v1.25.4.2, you can encrypt the password saved in `bin/steamcmd/userData.txt` for the Windows account that runs the app. Automatic updates can use the encrypted password. Steam Guard can still ask for confirmation until SteamCMD has a saved login for that account.

## Stop repeated Steam Guard prompts

WGSM saves your password, but the login session that satisfies Steam Guard belongs to SteamCMD. The password helper below creates it: after saving the password, it connects to Steam once, asks for your Steam Guard code if needed and checks that SteamCMD kept the login.

If the helper reports that no login was saved, create it by hand:

1. Close WindowsGSM and open a console in `bin\steamcmd`.
2. Run `steamcmd.exe`, enter `login YourSteamLogin`, then your password and Steam Guard code.
3. Enter `quit`.

Before each account login, WGSM tests the saved session without a password. If that works, it logs in with the username only and the SteamCMD console shows `Logging in using cached credentials`. If the session is missing, expired or the test takes longer than two minutes, WGSM uses the saved password as before, so Steam Guard can ask again. The test adds a few seconds to each account update.

## Set up the password helper

1. Download the Windows x64 ZIP for the setup helper. [Latest MeFriendos release](https://github.com/PapaGordon/WindowsGSM/releases/latest)
2. Close WindowsGSM and extract the ZIP into your WindowsGSM folder. Keep your existing `servers`, `configs`, `plugins` and `bin` folders.
3. Log into Windows as the same user that normally runs WindowsGSM. If WGSM runs under another service or scheduled-task account, the helper must run under that account too, with its user profile loaded.
4. Double-click **Encrypt-SteamPassword.cmd** in the WindowsGSM folder.
5. Enter your **Steam login name**, then your password. The password is hidden while you type. Press Enter when done.
6. Wait for **Encrypted credentials saved for the current Windows user**. Enter your Steam Guard code if SteamCMD asks for it.
7. Wait for **Steam login saved**. You can then close the window and start WindowsGSM again.

The helper asks for the password locally, saves it and connects to Steam once. While that one login runs, SteamCMD receives the password as a process argument. Add `-SkipSteamLogin` when running the PowerShell script directly to save the password without connecting. If `userData.txt` does not exist yet, open the server installation screen in WindowsGSM and click **Set Account**. Close WindowsGSM and run the helper again.

The CMD starts only the included PowerShell script. Its execution-policy setting applies to that one PowerShell process and does not change the machine's saved policy.

## What changes in the file?

The helper keeps your username and comments, empties the old `steamPass` field and adds `steamPassEncrypted`. That long value is the encrypted password. Do not paste it into `steamPass`.

No plaintext backup is created. Old copies or backups of your previous `userData.txt` can still contain the readable password, so check those yourself. The helper never prints your password.

Keep using the same Windows account on the same machine. Run the helper again when you move WindowsGSM to a different machine or Windows account, or when you change the Steam password.

## If you prefer PowerShell

This example runs the helper from a WindowsGSM installation in `F:\WindowsGSM`.

```powershell
Set-Location 'F:\WindowsGSM'
powershell.exe -NoProfile -ExecutionPolicy Bypass -File '.\tools\Set-SteamCredentials.ps1' -UserDataPath 'F:\WindowsGSM\bin\steamcmd\userData.txt' -SteamUser 'YourSteamLogin'
```

Replace `YourSteamLogin` with your Steam login name and adjust the path if needed. Enter the password only at the hidden prompt, never in the command.

## Limits and compatibility

This protects against someone simply opening or copying the account file. It does not protect against an administrator or software running as your Windows user. When no saved SteamCMD login works, WGSM passes the decrypted password to SteamCMD as a process argument, which suitably privileged processes can read while SteamCMD runs. The `bin\steamcmd\config\config.vdf` file holds the saved login, so protect it like a password and do not share it. Do not share process listings or logs containing those arguments.

Use a WindowsGSM build with encrypted-password support before converting the file. Older builds and third-party plugins that read `steamPass` themselves cannot use the encrypted field. Our Arma 3, DayZ and Farming Simulator 22/25 plugins use the central SteamCMD installer and accept the encrypted login on MeFriendos v1.25.4.2 or newer. Only SteamCMD servers that require an account need a saved password.

The DayZAuto source supplied in our v1.25.4.3 plugin SDK also uses the central credential reader. Older copies of that example still read only plaintext and need updating separately.

DayZ and Arma 3 can use the same saved account to update a configured list of mods before each start. [Workshop updates](workshop.md)

Legacy plaintext files still work. When `steamPassEncrypted` is present, WGSM uses it and will not fall back to plaintext if decryption fails.

## Storage format for developers

`steamPassEncrypted` stores UTF-8 password bytes encrypted with Windows DPAPI in CurrentUser scope, without optional entropy. The result is encoded as Base64. Use the included helper to write this format. PowerShell's `ConvertFrom-SecureString` uses a different format. Local checks cover the helper and WindowsGSM reader together.
