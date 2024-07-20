using Serilog;
using StrictParent.Service.Services;
using System;
using System.ServiceProcess;

namespace StrictParent.Service
{
    public partial class StrictParentService : ServiceBase
    {
        public StrictParentService(ILogger logger, OrchestratorService orchestratorService)
        {
            InitializeComponent();

            _logger = logger;

            _orchestratorService = orchestratorService;
        }

        private readonly ILogger _logger;
        private OrchestratorService _orchestratorService;
        internal void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            _logger.Information("OnStart Called");

            base.OnStart(args);

            try
            {
                _orchestratorService.Initialize();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while initializing the service");
            }
        }

        protected override void OnShutdown()
        {
            _logger.Information("OnShutdown Called");

            _orchestratorService.Stop();

            base.OnShutdown();
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            _logger.Information($"OnSessionChange Called, Reason = {changeDescription.Reason}");

            _orchestratorService.HandleSessionChanged(changeDescription.Reason);

            base.OnSessionChange(changeDescription);
        }

        protected override Boolean OnPowerEvent(PowerBroadcastStatus powerStatus)
        {

            switch (powerStatus)
            {
                case PowerBroadcastStatus.OemEvent:
                case PowerBroadcastStatus.Suspend:
                case PowerBroadcastStatus.ResumeSuspend:
                case PowerBroadcastStatus.QuerySuspend:
                case PowerBroadcastStatus.QuerySuspendFailed:
                case PowerBroadcastStatus.ResumeAutomatic:
                case PowerBroadcastStatus.ResumeCritical:

                    _logger.Information($"OnPowerEvent Called, PowerStatus = {powerStatus}");

                    break;
                default:
                    break;
            }

            return base.OnPowerEvent(powerStatus);
        }
    }
}