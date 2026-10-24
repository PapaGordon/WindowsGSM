// Made by PapaGordon
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static class Program
{
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr window, int state);

    [STAThread]
    private static void Main(string[] args)
    {
        string marker = args.First(value => value.StartsWith("-fixture=")).Substring(9);
        string profiles = args.FirstOrDefault(value => value.StartsWith("-profiles="))?.Substring(10);
        string rpt = profiles == null ? null : Path.Combine(profiles, "DayZServer_fixture.rpt");
        using (var form = new Form { Text="WindowsGSM window fixture", StartPosition=FormStartPosition.Manual, Location=new Point(-30000, -30000) })
        {
            form.Shown += (sender, e) => {
                ShowWindow(form.Handle, 0);
                File.WriteAllText(marker, form.Handle.ToInt64().ToString());
                if (rpt != null) File.AppendAllText(rpt, "fixture startup\r\n");
            };
            form.FormClosed += (sender, e) => {
                if (rpt != null) File.AppendAllText(rpt, "fixture shutdown\r\n");
            };
            Application.Run(form);
        }
    }
}
