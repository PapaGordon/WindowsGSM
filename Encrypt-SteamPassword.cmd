@echo off
rem Made by PapaGordon
echo Save an encrypted Steam password for WindowsGSM.
echo Run this as the same Windows user that runs WindowsGSM.
echo.
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0tools\Set-SteamCredentials.ps1" -UserDataPath "%~dp0bin\steamcmd\userData.txt"
set "wgsmExitCode=%ERRORLEVEL%"
echo.
if not "%wgsmExitCode%"=="0" echo Could not save the credentials. Read the error above and STEAM-CREDENTIALS.md.
pause
exit /b %wgsmExitCode%
