using Serilog;
using StrictParent.Common;
using StrictParent.Common.DTOs;
using StrictParent.Service.Services;
using System;
using System.Threading.Tasks;

namespace StrictParent.Service
{
    internal class WCFContract : IWCF
    {
        public WCFContract(ILogger logger, OrchestratorService orchestratorService, AppStatusService appStatusService)
        {
            _logger = logger;
            _orchestratorService = orchestratorService;
            _appStatusService = appStatusService;
        }

        private readonly ILogger _logger;
        private readonly OrchestratorService _orchestratorService;
        private readonly AppStatusService _appStatusService;
        public SettingsDto GetSettings()
        {
            try
            {
                _logger.Information("GetSettings called from WCF Client");

                ISettings s = DataAccess.Settings.LoadSettings();

                _logger.Information($"Settings Retrieved -> From = '{s.UnplugFrom}', Until = '{s.UnplugUntil}', TimeZone = '{s.TimeZoneID}', Unkillable = '{s.Unkillable}', RunUntil = '{s.RunUntil}'");

                return new SettingsDto()
                {
                    RunUntil = s.RunUntil,
                    TimeZoneID = s.TimeZoneID,
                    Unkillable = s.Unkillable,
                    UnplugFrom = s.UnplugFrom,
                    UnplugUntil = s.UnplugUntil
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while a WCF client called GetSettings");

                return new SettingsDto();
            }
        }

        public void SaveSettings(SettingsDto settings)
        {
            try
            {
                _logger.Information($"SaveSettings called from WCF Client. Saving Settings -> {settings}");

                ISettings s = settings;

                DataAccess.Settings.SaveSettings(s);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while a WCF client called SaveSettings");
            }
        }

        public void SettingsChanged()
        {
            try
            {
                _logger.Information("SettingsChanged called from WCF Client");

                _orchestratorService.Refresh();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while a WCF client called SettingsChanged");
            }
        }
        public async Task<StatusResponseDto> CheckStatus(SettingsDto settings)
        {
            var statusResponse = await _appStatusService.CheckStatus(settings);

            return new StatusResponseDto()
            {
                Status = (int)statusResponse.Status,
                TimeStamp = statusResponse.TimeStamp,
                Interval = statusResponse.Interval
            };
        }

        public DateTime[] ParseCorrectDateTime(DateTime now, string fromTimeStr, string untilTimeStr) =>
            _appStatusService.ParseCorrectDateTime(now, fromTimeStr, untilTimeStr);
    }
}