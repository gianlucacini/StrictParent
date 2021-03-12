using Serilog;
using Unplug.BusinessLayer;
using Unplug.Common;

namespace Unplug.Service
{
    internal class WCF : IWCF
    {
        public SettingsDto GetSettings()
        {
            try
            {
                Log.Information("GetSettings called from WCF Client");

                ISettings s = DataAccess.Settings.LoadSettings();

                Log.Information($"Settings Retrieved -> From = '{s.UnplugFrom}', Until = '{s.UnplugUntil}', TimeZone = '{s.TimeZoneID}', Unkillable = '{s.Unkillable}', RunUntil = '{s.RunUntil}'");

                return new SettingsDto()
                {
                    RunUntil = s.RunUntil,
                    TimeZoneID = s.TimeZoneID,
                    Unkillable = s.Unkillable,
                    UnplugFrom = s.UnplugFrom,
                    UnplugUntil = s.UnplugUntil
                };
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "An error occurred while a WCF client called GetSettings");

                return new SettingsDto();
            }
        }

        public void SaveSettings(SettingsDto settings)
        {
            try
            {
                Log.Information($"SaveSettings called from WCF Client. Saving Settings -> {settings}");

                ISettings s = settings;

                DataAccess.Settings.SaveSettings(s);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "An error occurred while a WCF client called SaveSettings");
            }
        }

        public void SettingsChanged()
        {
            try
            {
                Log.Information("SettingsChanged called from WCF Client");
                
                ServiceHelper.Refresh();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "An error occurred while a WCF client called SettingsChanged");
            }
        }
    }
}