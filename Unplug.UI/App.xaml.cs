using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Unplug.UI
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(Object sender, StartupEventArgs e)
        {
            bool anotherClientInstanceIsRunning = System.Diagnostics.Process.GetProcessesByName(System.IO.Path
   .GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;

            if (anotherClientInstanceIsRunning)
            {
                //already an instance running
                Environment.Exit(0);
            }
            else
            {
                ServiceConnection.Init();
            }
        }
    }
}
