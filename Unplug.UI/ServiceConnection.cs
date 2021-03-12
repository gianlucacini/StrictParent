using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unplug.UI
{
    public class ServiceConnection
    {
        public static void Init()
        {
            var processes = System.Diagnostics.Process.GetProcessesByName("QuitSurf.Service.exe");

            if (processes.Length == 0)
                ManuallyStartQuitSurfService();

        }

        private static void ManuallyStartQuitSurfService()
        {
            try
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/C net start \"Unplug Service\"",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };

                System.Diagnostics.Process process = new System.Diagnostics.Process
                {
                    StartInfo = startInfo
                };

                process.Start();

                process.WaitForExit();

            }
            catch (Exception ex)
            {

            }
        }
    }
}
