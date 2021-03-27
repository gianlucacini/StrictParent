using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Unplug.Common;

namespace Unplug.BusinessLayer
{
    public static class UnplugJob
    {
        static Timer UnplugTimer;
        static ISettings _settings;

        public static void Begin(ISettings settings)
        {
            Log.Information($"UnplugJob Starting with the following settings: " +
                $"From = '{settings.UnplugFrom}', " +
                $"Until = '{settings.UnplugUntil}', " +
                $"Run Until '{settings.RunUntil}', " +
                $"TimeZone = '{settings.TimeZoneID}', " +
                $"Unkillable = '{settings.Unkillable}'");

            if (settings.RunUntil <= DateTime.Now)
            {
                End();
                return;
            }

            _settings = settings;

            UnplugTimer = new Timer
            {
                AutoReset = false,
                Interval = 1 * 1000
            };

            UnplugTimer.Elapsed += UnplugTimer_Elapsed;
            
            UnplugTimer.Start();

            Log.Information("UnplugJob Started = " + UnplugTimer.Enabled);

        }

        private static void UnplugTimer_Elapsed(Object sender, ElapsedEventArgs e)
        {
            Log.Information("UnplugJob Elapsed, Checking Status...");

            var response = AppStatusService.CheckStatus(_settings).Result;

            CriticalProcessBase.StatusChanged(response);

            Log.Information("UnplugJob Responded With: " + response.ToString());

            if(response.Interval == null)
            {
                End();

                return;
            }


            if(response.Status == AppStatus.Offline)
            {
                FirewallJob.Begin();
            }
            else if(response.Status == AppStatus.Online)
            {
                FirewallJob.End();
            }
            else
            {
                Log.Error("Could Not Retrieve Date Time From NTP. Checking again in 30 seconds...");
            }

            UnplugTimer.Interval = response.Interval.Value;
            UnplugTimer.Start();
        }

        public static void End()
        {
            Log.Information("UnplugJob Ending");

            UnplugTimer?.Stop();
            UnplugTimer?.Dispose();
            UnplugTimer?.Close();
        }
    }
}
