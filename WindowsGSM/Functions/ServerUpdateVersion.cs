// Made by PapaGordon
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WindowsGSM.Functions
{
    internal static class ServerUpdateVersion
    {
        internal static async Task WaitForCompletion(Task update)
        {
            await update;
            var result = update.GetType().GetProperty("Result");
            var process = result == null ? null : result.GetValue(update) as Process;
            if (process != null) await Task.Run(() => process.WaitForExit());
        }

        internal static string InstalledOrReported(object server, string reported)
        {
            try
            {
                var method = server.GetType().GetMethod("GetLocalBuild", Type.EmptyTypes);
                string installed = method == null ? null : method.Invoke(server, null) as string;
                if (!string.IsNullOrWhiteSpace(installed) && installed != "unknown") return installed;
            }
            catch { /* Keep the reported version when the local probe is unavailable. */ }
            return reported;
        }
    }
}
