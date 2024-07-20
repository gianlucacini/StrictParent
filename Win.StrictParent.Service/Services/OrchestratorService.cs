using Serilog;
using StrictParent.Common;
using StrictParent.DataAccess;
using StrictParent.Service.BusinessLogic;
using StrictParent.Service.Jobs;
using System;
using System.Security;
using System.ServiceProcess;

namespace StrictParent.Service.Services
{
    public class OrchestratorService
    {
        public OrchestratorService(
            ILogger logger,
            BlockConnectionsFirewallRule firewallRule,
            RegistryWrapper registryWrapper,
            StrictParentJob strictParentJob,
            CriticalProcess criticalProcess)
        {
            _logger = logger;
            _firewallRule = firewallRule;
            _registryWrapper = registryWrapper;
            _strictParentJob = strictParentJob;
            _criticalProcess = criticalProcess;
        }

        private readonly ILogger _logger;
        private readonly BlockConnectionsFirewallRule _firewallRule;
        private readonly RegistryWrapper _registryWrapper;
        private readonly StrictParentJob _strictParentJob;
        private readonly CriticalProcess _criticalProcess;
        public void Initialize()
        {
            _logger.Information("ServiceHelper Initialize called");

            if (UserHasAdminPrivileges() == false)
            {
                throw new SecurityException("User has no admin privileges. Application cannot start");
            }

            ISettings s = Settings.LoadSettings();

            _firewallRule.AllowConnection(true);

            _registryWrapper.StartRegistryMonitor();

            _strictParentJob.Begin(s);
        }

        public void Stop()
        {
            _logger.Information("ServiceHelper Stop called");

            ISettings s = Settings.LoadSettings();

            _criticalProcess.SetProcessAsNotCritical(s);

            _firewallRule.AllowConnection(true);

            _registryWrapper.StopRegistryMonitor();

            _registryWrapper.RestoreDefaultSettings(true);
        }

        public void Refresh()
        {
            _logger.Information("ServiceHelper Refresh called");

            _registryWrapper.RestoreDefaultSettings(true);

            ISettings s = Settings.LoadSettings();

            _strictParentJob.End();

            _strictParentJob.Begin(s);
        }
        public void HandleSessionChanged(SessionChangeReason reason)
        {
            _logger.Information("ServiceHelper HandleSessionChanged called");

            Refresh();
        }

        public Boolean UserHasAdminPrivileges()
        {
            _logger.Information("Checking if current user has admin privileges");

            using (System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);

                Boolean isAdmin = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

                _logger.Information($"{principal.Identity.Name} is admin = {isAdmin}");

                return isAdmin;
            }
        }
    }
}