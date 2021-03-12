using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unplug.Service
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        static void Main()
        {
            String localPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            String logPath = System.IO.Path.Combine(localPath, $"Unplug_{DateTime.Now.ToString("yyyy")}_{DateTime.Now.ToString("MM")}.log");

            Log.Logger = new LoggerConfiguration().WriteTo.File(logPath).CreateLogger();

#if DEBUG

            Log.Information("Service is Starting in Debug Mode");

            new UPService().OnDebug(null);
            
            Thread.Sleep(Timeout.Infinite);
#else

            if (Environment.UserInteractive)
            {
                Log.Information("Service cannot start in Interactive Mode");
                return;
            }

            Log.Information("Service is Starting in Production Mode");

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new UPService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
