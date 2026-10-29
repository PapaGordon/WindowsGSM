# Save your Steam password encrypted

You no longer have to leave the Steam password readable in `bin/steamcmd/userData.txt`. WindowsGSM v1.25.4.2 can read a password encrypted for the Windows user running WGSM. Automatic updates can still use it, provided Steam does not ask for a new Steam Guard confirmation.

## The easy way

1. Download the **WindowsGSM-v1.25.4.2-win-x64.zip** release. The EXE alone does not include the setup helper.
2. Quit WindowsGSM before replacing its EXE. Extract the ZIP into your WindowsGSM folder. Keep your existing `servers`, `configs`, `plugins` and `bin` folders. The ZIP contains no Steam account file or server data to overwrite.
3. Log into Windows as the same user that normally runs WindowsGSM. If WGSM runs under another service or scheduled-task account, the helper must run under that account too, with its user profile loaded.
4. Double-click **Encrypt-SteamPassword.cmd** in the WindowsGSM folder.
5. Enter your **Steam login name**, then your password. The password is hidden while you type. Press Enter when done.
6. Wait for **Encrypted credentials saved for the current Windows user**. You can then close the window and start WindowsGSM again.

The helper asks for the password locally. It does not sign into Steam or change Steam Guard. If `userData.txt` does not exist yet, open the server installation screen in WGSM, click **Set Account** once to create it, then close WGSM and run the helper again. You do not need to put your password into Notepad first.

The CMD starts only the included PowerShell script. Its execution-policy setting applies to that one PowerShell process and does not change the machine's saved policy.

## What changes in the file?

The helper keeps your username and comments, empties the old `steamPass` field and adds `steamPassEncrypted`. That long value is the encrypted password. Do not paste it into `steamPass`.

No plaintext backup is created. Old copies or backups of your previous `userData.txt` can still contain the readable password, so check those yourself. The helper never prints your password.

Keep using the same Windows account on the same server. If you move WGSM to another machine or Windows account, run the helper again there and enter the password again. Copying the encrypted file alone is not enough. To change the password later, run the helper again.

## If you prefer PowerShell

For an installation in `F:\WindowsGSM`, open PowerShell and run:

```powershell
Set-Location 'F:\WindowsGSM'
powershell.exe -NoProfile -ExecutionPolicy Bypass -File '.\tools\Set-SteamCredentials.ps1' -UserDataPath 'F:\WindowsGSM\bin\steamcmd\userData.txt' -SteamUser 'YourSteamLogin'
```

Replace `YourSteamLogin` with your Steam login name and adjust the path if needed. Enter the password only at the hidden prompt, never in the command.

## Limits and compatibility

This protects against someone simply opening or copying the account file. It does not protect against an administrator or software running as your Windows user. WGSM still passes the decrypted password to SteamCMD as a process argument, which suitably privileged processes can read while SteamCMD runs. Do not share process listings or logs containing those arguments.

Use the new WGSM version before encrypting the file. Older WGSM builds and third-party plugins that read `steamPass` themselves cannot use the encrypted field. Our current DayZ plugin uses WGSM's installer for server updates and needs no separate credential reader. The older DayZAuto example is a different plugin.

Steam Guard can still interrupt an unattended update. Also, DayZ server updates and Workshop mod updates are separate: the current built-in DayZ module does not automatically download Workshop mods.

Legacy plaintext files still work. When `steamPassEncrypted` is present, WGSM uses it and will not fall back to plaintext if decryption fails.

## Storage format for developers

`steamPassEncrypted` contains a Base64-encoded Windows DPAPI blob: UTF-8 password bytes, CurrentUser scope, no optional entropy. Do not replace it with the output of PowerShell's `ConvertFrom-SecureString`; that uses a different serialized format. Local checks cover the setup helper and WGSM reader together.
