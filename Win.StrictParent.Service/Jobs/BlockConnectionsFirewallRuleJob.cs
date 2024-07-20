using Serilog;
using StrictParent.Service.BusinessLogic;
using System;
using System.Timers;

namespace StrictParent.Service.Jobs
{
    public class BlockConnectionsFirewallRuleJob
    {
        public BlockConnectionsFirewallRuleJob(ILogger logger, BlockConnectionsFirewallRule firewallRule)
        {
            _logger = logger;
            _firewallRule = firewallRule;
        }

        private readonly BlockConnectionsFirewallRule _firewallRule;
        private readonly ILogger _logger;
        private Timer FirewallTimer;

        public void Begin()
        {
            _logger.Information("FirewallJob Starting");

            FirewallTimer = new Timer
            {
                AutoReset = true,
                Interval = 10 * 1000
            };

            FirewallTimer.Elapsed += FirewallTimer_Elapsed;

            FirewallTimer.Start();

            _logger.Information("FirewallJob Started = " + FirewallTimer.Enabled);
        }

        private void FirewallTimer_Elapsed(Object sender, ElapsedEventArgs e)
        {
            _firewallRule.EnableFirewallIfDown();
            _firewallRule.AllowConnection(false);
            _firewallRule.DenyConnection(false);
        }

        public void End()
        {
            _logger.Information("FirewallJob Ending");

            _firewallRule.AllowConnection(true);

            FirewallTimer?.Stop();
            FirewallTimer?.Dispose();
            FirewallTimer?.Close();
        }
    }
}