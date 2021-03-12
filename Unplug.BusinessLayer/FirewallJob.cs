using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Unplug.BusinessLayer
{
    public class FirewallJob 
    {
        private Timer FirewallTimer;

        public void Begin()
        {
            Log.Information("FirewallJob Starting");

            FirewallTimer = new Timer
            {
                AutoReset = true,
                Interval = 10 * 1000
            };

            FirewallTimer.Elapsed += FirewallTimer_Elapsed;

            FirewallTimer.Start();

            Log.Information("FirewallJob Started = " + FirewallTimer.Enabled);

        }

        private void FirewallTimer_Elapsed(Object sender, ElapsedEventArgs e)
        {
            FirewallRule.EnableFirewallIfDown();
            FirewallRule.AllowConnection(false);
            FirewallRule.DenyConnection(false);
        }

        public void End()
        {
            Log.Information("FirewallJob Ending");

            FirewallRule.AllowConnection(true);
            
            FirewallTimer?.Stop();
            FirewallTimer?.Dispose();
            FirewallTimer?.Close();
        }
    }
}
