# Made by PapaGordon
# Run on the server as the Windows user that runs WindowsGSM.
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$UserDataPath,
    [string]$SteamUser,
    [switch]$SkipSteamLogin
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Security

function Start-SteamCmd([string]$SteamCmdPath, [string]$Arguments, [bool]$Capture) {
    $info = [Diagnostics.ProcessStartInfo]::new($SteamCmdPath, $Arguments)
    $info.WorkingDirectory = [IO.Path]::GetDirectoryName($SteamCmdPath)
    $info.UseShellExecute = $false
    $info.RedirectStandardOutput = $Capture
    $info.RedirectStandardInput = $Capture
    $process = [Diagnostics.Process]::Start($info)
    try {
        if (!$Capture) { $process.WaitForExit(); return '' }
        $process.StandardInput.Close()
        $output = $process.StandardOutput.ReadToEndAsync()
        if (!$process.WaitForExit(120000)) { $process.Kill(); return '' }
        return $output.GetAwaiter().GetResult()
    }
    finally { $process.Dispose() }
}

# SteamCMD keeps the login in its own cache, so WGSM can update later without another Steam Guard code.
function Connect-SteamOnce([string]$SteamCmdPath, [string]$SteamUser, [string]$Password) {
    $manual = "Log in once by hand: run steamcmd.exe in $([IO.Path]::GetDirectoryName($SteamCmdPath)), then enter: login $SteamUser"
    if ($SkipSteamLogin) { return }
    if (![IO.File]::Exists($SteamCmdPath)) {
        Write-Host "SteamCMD is not installed yet, so no Steam login was performed. $manual"
        return
    }
    if ($Password.Contains('"')) {
        Write-Host "This password cannot be passed to SteamCMD automatically. $manual"
        return
    }
    try {
        Write-Host 'Connecting to Steam once to save the login. Enter your Steam Guard code if SteamCMD asks for it.'
        [void](Start-SteamCmd $SteamCmdPath "+login `"$SteamUser`" `"$Password`" +quit -overrideminos" $false)
        Write-Host 'Checking the saved login...'
        $output = Start-SteamCmd $SteamCmdPath "+@ShutdownOnFailedCommand 1 +@NoPromptForPassword 1 +login `"$SteamUser`" +quit -overrideminos" $true
        if ($output -match 'Logging in using cached credentials' -and $output -match '(to Steam Public|Waiting for user info)\s*\.+\s*OK') {
            Write-Host 'Steam login saved. WindowsGSM can now update without another Steam Guard code.'
        }
        else {
            Write-Host "The password is saved, but SteamCMD did not keep the login. $manual"
        }
    }
    catch {
        Write-Host "The password is saved, but SteamCMD could not be started. $manual"
    }
}

$path = [IO.Path]::GetFullPath($UserDataPath)
if (!(Test-Path -LiteralPath $path -PathType Leaf)) { throw 'The existing userData.txt was not found.' }
$steamCmd = Join-Path ([IO.Path]::GetDirectoryName($path)) 'steamcmd.exe'
if ([string]::IsNullOrWhiteSpace($SteamUser)) { $SteamUser = Read-Host 'Steam login name (not your display name)' }
if ($SteamUser.IndexOfAny([char[]]@('"', "`r", "`n")) -ge 0 -or [string]::IsNullOrWhiteSpace($SteamUser)) {
    throw 'Enter a Steam login name without quotes or line breaks.'
}
$password = Read-Host 'Steam password' -AsSecureString
if ($password.Length -eq 0) { throw 'The password must not be empty.' }
$pointer = [IntPtr]::Zero
$clear = $null
$temporary = $path + '.' + [Guid]::NewGuid().ToString('N') + '.tmp'
try {
    $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)
    $clear = [Text.Encoding]::UTF8.GetBytes([Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer))
    $cipher = [Security.Cryptography.ProtectedData]::Protect($clear, $null, [Security.Cryptography.DataProtectionScope]::CurrentUser)
    # Keep comments and other settings, but never write a plaintext backup.
    $lines = @([IO.File]::ReadAllLines($path) | Where-Object { $_ -notmatch '^\s*steam(User|Pass|PassEncrypted)\s*=' })
    $lines += 'steamUser="' + $SteamUser + '"'
    $lines += 'steamPass=""'
    $lines += 'steamPassEncrypted="' + [Convert]::ToBase64String($cipher) + '"'
    [IO.File]::WriteAllLines($temporary, [string[]]$lines, [Text.UTF8Encoding]::new($false))
    [IO.File]::Replace($temporary, $path, [System.Management.Automation.Language.NullString]::Value)
    Write-Host 'Encrypted credentials saved for the current Windows user.'
    Connect-SteamOnce -SteamCmdPath $steamCmd -SteamUser $SteamUser -Password ([Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer))
}
finally {
    if ($pointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer) }
    if ($null -ne $clear) { [Array]::Clear($clear, 0, $clear.Length) }
    $password.Dispose()
    if ([IO.File]::Exists($temporary)) { [IO.File]::Delete($temporary) }
}
