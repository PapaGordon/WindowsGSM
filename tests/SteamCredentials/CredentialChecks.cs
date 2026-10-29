// Made by PapaGordon
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using WindowsGSM.Installer;

internal static class CredentialChecks
{
    private static int passed;
    private static void Check(bool value, string message)
    {
        if (!value) throw new Exception(message);
        passed++;
        Console.WriteLine("PASS " + message);
    }

    private static void Main()
    {
        string root = Path.Combine(AppContext.BaseDirectory, "credentials-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        string path = Path.Combine(root, "userData.txt");
        try
        {
            Check(SteamCredentials.Read(path) == (null, null), "Missing file fails without credentials");
            File.WriteAllText(path, "\n/\n// comment\nbroken\nsteamUser = \"test-user\"\nsteamPass=\"test=päss & %!+\"\n");
            Check(SteamCredentials.Read(path) == ("test-user", "test=päss & %!+"), "Legacy parser handles blank lines, equals signs and special characters");
            byte[] cipher = ProtectedData.Protect(Encoding.UTF8.GetBytes("encrypted-päss=123"), null, DataProtectionScope.CurrentUser);
            string encrypted = Convert.ToBase64String(cipher);
            File.AppendAllText(path, "steamPassEncrypted=\"" + encrypted + "\"\n");
            Check(SteamCredentials.Read(path) == ("test-user", "encrypted-päss=123"), "CurrentUser DPAPI round trip takes precedence over legacy plaintext");
            File.AppendAllText(path, "steamPassEncrypted=\"damaged!\"\n");
            Check(SteamCredentials.Read(path) == (null, null), "Invalid Base64 cannot fall back to plaintext");
            File.WriteAllText(path, "steamUser=test-user\nsteamPass=legacy\nsteamPassEncrypted=\"\"\n");
            Check(SteamCredentials.Read(path) == (null, null), "Empty encrypted entry fails closed");
            cipher[cipher.Length - 1] ^= 1;
            File.WriteAllText(path, "steamUser=test-user\nsteamPassEncrypted=" + Convert.ToBase64String(cipher));
            Check(SteamCredentials.Read(path) == (null, null), "Tampered DPAPI blob is rejected");

            string setup = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../tools/Set-SteamCredentials.ps1"));
            File.WriteAllText(path, "// retain comment\nother=value\nsteamUser=old\nsteamPass=old-plaintext\nsteamPassEncrypted=old\n");
            string wrapper = Path.Combine(root, "setup-check.ps1");
            File.WriteAllText(wrapper, "param($Setup, $Target)\n$ErrorActionPreference = 'Stop'\nfunction Read-Host { param($Prompt, [switch]$AsSecureString) if (!$AsSecureString) { return 'test-user' }; $secure = [Security.SecureString]::new(); foreach ($c in 'dummy-test=päss123'.ToCharArray()) { $secure.AppendChar($c) }; $secure }\n& $Setup -UserDataPath $Target\n", new UTF8Encoding(true));
            using (var process = new Process { StartInfo = new ProcessStartInfo("powershell.exe") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true } })
            {
                foreach (string arg in new[] { "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", wrapper, setup, path }) process.StartInfo.ArgumentList.Add(arg);
                process.Start();
                var output = process.StandardOutput.ReadToEndAsync();
                var error = process.StandardError.ReadToEndAsync();
                if (!process.WaitForExit(30000)) { process.Kill(); throw new Exception("Setup script timed out"); }
                Console.Write(output.GetAwaiter().GetResult());
                Console.Error.Write(error.GetAwaiter().GetResult());
                Check(process.ExitCode == 0, "Windows PowerShell setup script completes with a mock password prompt");
            }
            string contents = File.ReadAllText(path);
            Check(SteamCredentials.Read(path) == ("test-user", "dummy-test=päss123"), "PowerShell encryption interoperates with the WGSM reader");
            Check(contents.Contains("// retain comment") && contents.Contains("other=value"), "Setup preserves comments and other settings");
            Check(!contents.Contains("old-plaintext") && !contents.Contains("dummy-test") && contents.Contains("steamPass=\"\""), "Setup removes plaintext from the file");
            Check(Directory.GetFiles(root).Length == 2, "Setup creates neither a backup nor temporary leftovers");
            Console.WriteLine(passed + " credential checks passed.");
        }
        finally { Directory.Delete(root, true); }
    }
}
