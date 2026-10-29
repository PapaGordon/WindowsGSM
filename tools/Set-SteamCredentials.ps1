# Made by PapaGordon
# Run on the server as the Windows user that runs WindowsGSM.
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$UserDataPath,
    [string]$SteamUser
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Security

$path = [IO.Path]::GetFullPath($UserDataPath)
if (!(Test-Path -LiteralPath $path -PathType Leaf)) { throw 'The existing userData.txt was not found.' }
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
    Write-Host 'Encrypted credentials saved for the current Windows user. No Steam login was performed.'
}
finally {
    if ($pointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer) }
    if ($null -ne $clear) { [Array]::Clear($clear, 0, $clear.Length) }
    $password.Dispose()
    if ([IO.File]::Exists($temporary)) { [IO.File]::Delete($temporary) }
}
