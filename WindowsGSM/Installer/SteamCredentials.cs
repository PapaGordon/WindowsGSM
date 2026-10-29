// Made by PapaGordon
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WindowsGSM.Installer
{
    public static class SteamCredentials
    {
        public static (string, string) Read(string path)
        {
            try
            {
                if (!File.Exists(path)) return (null, null);
                string username = null, password = null, encrypted = null;
                foreach (string raw in File.ReadLines(path))
                {
                    string line = raw.Trim();
                    if (line.Length == 0 || line.StartsWith("//", StringComparison.Ordinal)) continue;
                    int equals = line.IndexOf('=');
                    if (equals < 0) continue;
                    string key = line.Substring(0, equals).Trim();
                    string value = line.Substring(equals + 1).Trim();
                    if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
                        value = value.Substring(1, value.Length - 2);
                    switch (key)
                    {
                        case "steamUser": username = value; break;
                        case "steamPass": password = value; break;
                        case "steamPassEncrypted": encrypted = value; break;
                    }
                }
                if (encrypted != null)
                {
                    // Never fall back to a plaintext password if decryption fails.
                    byte[] clear = ProtectedData.Unprotect(Convert.FromBase64String(encrypted), null, DataProtectionScope.CurrentUser);
                    try { password = Encoding.UTF8.GetString(clear); }
                    finally { Array.Clear(clear, 0, clear.Length); }
                }
                return string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)
                    ? (null, null) : (username, password);
            }
            catch (IOException) { return (null, null); }
            catch (UnauthorizedAccessException) { return (null, null); }
            catch (CryptographicException) { return (null, null); }
            catch (FormatException) { return (null, null); }
        }
    }
}
