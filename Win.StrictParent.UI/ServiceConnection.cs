using System;

namespace StrictParent.UI
{
    public class ServiceConnection
    {
        public static void Init()
        {
            var processes = System.Diagnostics.Process.GetProcessesByName("StrictParent.Service");

            if (processes.Length == 0)
                ManuallyStartStrictParentService();

        }

        private static void ManuallyStartStrictParentService()
        {
            try
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/C net start \"StrictParent Service\"",
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
