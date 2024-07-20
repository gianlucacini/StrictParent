using Serilog;
using StrictParent.Common;
using StrictParent.Common.Models;
using StrictParent.Service.BusinessLogic;
using StrictParent.Service.Services;
using System;
using System.Timers;

namespace StrictParent.Service.Jobs
{
    public class StrictParentJob
    {
        public StrictParentJob(
            ILogger logger,
            AppStatusService appStatusService,
            CriticalProcess criticalProcess,
            BlockConnectionsFirewallRuleJob firewallJob)
        {
            _logger = logger;
            _appStatusService = appStatusService;
            _criticalProcess = criticalProcess;
            _firewallJob = firewallJob;
        }

        private readonly ILogger _logger;
        private readonly AppStatusService _appStatusService;
        private readonly CriticalProcess _criticalProcess;
        private readonly BlockConnectionsFirewallRuleJob _firewallJob;
        static Timer StrictParentTimer;
        static ISettings _settings;

        public void Begin(ISettings settings)
        {
            _logger.Information($"StrictParent Job Starting with the following settings: " +
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

            StrictParentTimer = new Timer
            {
                AutoReset = false,
                Interval = 1 * 1000
            };

            StrictParentTimer.Elapsed += StrictParentTimer_Elapsed;

            StrictParentTimer.Start();

            _logger.Information("StrictParent Job Started = " + StrictParentTimer.Enabled);
        }

        private void StrictParentTimer_Elapsed(Object sender, ElapsedEventArgs e)
        {
            _logger.Information("StrictParent Job Elapsed, Checking Status...");

            var response = _appStatusService.CheckStatus(_settings).Result;

            _criticalProcess.StatusChanged(response);

            _logger.Information("StrictParent Job Responded With: " + response.ToString());

            if (response.Interval == null)
            {
                End();

                return;
            }

            if (response.Status == AppStatus.Offline)
            {
                _firewallJob.Begin();
            }
            else if (response.Status == AppStatus.Online)
            {
                _firewallJob.End();
            }
            else
            {
                _logger.Error("Could Not Retrieve Date Time From NTP. Checking again in 30 seconds...");
            }

            StrictParentTimer.Interval = response.Interval.Value;
            StrictParentTimer.Start();
        }

        public void End()
        {
            _logger.Information("StrictParent Job Ending");

            StrictParentTimer?.Stop();
            StrictParentTimer?.Dispose();
            StrictParentTimer?.Close();
        }
    }
}
