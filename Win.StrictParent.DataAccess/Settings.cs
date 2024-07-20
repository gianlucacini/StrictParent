using StrictParent.Common;
using System;

namespace StrictParent.DataAccess
{
    public class Settings
    {

        internal static void KeepFileOpen()
        {
            FileConfigToggler.ToggleFileConfigOpen(FileConfigAction.Open);
        }

        internal static void CloseFile()
        {
            FileConfigToggler.ToggleFileConfigOpen(FileConfigAction.Close);
        }

        public static ISettings LoadSettings()
        {
            CloseFile();

            var runUntil = ApplicationSettingsHelper.GetValue<long>("RunUntil");
            var timeZoneID = ApplicationSettingsHelper.GetValue<String>("TimeZoneID");
            var unplugFrom = ApplicationSettingsHelper.GetValue<String>("UnplugFrom");
            var unplugUntil = ApplicationSettingsHelper.GetValue<String>("UnplugUntil");
            var unkillable = ApplicationSettingsHelper.GetValue<Boolean>("Unkillable");

            KeepFileOpen();

            return new SettingsModel()
            {
                RunUntil = runUntil <= 0 ? DateTime.Now.AddDays(-1) : new DateTime(runUntil),
                TimeZoneID = timeZoneID,
                UnplugFrom = unplugFrom,
                UnplugUntil = unplugUntil,
                Unkillable = unkillable
            };
        }

        public static void SaveSettings(ISettings settings)
        {
            CloseFile();

            ApplicationSettingsHelper.UpdateAppSettings("RunUntil", settings.RunUntil.Ticks.ToString());
            ApplicationSettingsHelper.UpdateAppSettings("TimeZoneID", settings.TimeZoneID);
            ApplicationSettingsHelper.UpdateAppSettings("UnplugFrom", settings.UnplugFrom);
            ApplicationSettingsHelper.UpdateAppSettings("UnplugUntil", settings.UnplugUntil);
            ApplicationSettingsHelper.UpdateAppSettings("Unkillable", settings.Unkillable == true ? "1" : "0");

            KeepFileOpen();
        }
    }
}
