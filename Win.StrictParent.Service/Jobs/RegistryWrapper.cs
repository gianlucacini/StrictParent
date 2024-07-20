using Serilog;
using StrictParent.Service.BusinessLogic;
using System;

namespace StrictParent.Service.Jobs
{
    public class RegistryWrapper
    {
        public RegistryWrapper(ILogger logger)
        {
            _logger = logger;
        }

        private RegistryMonitor RegMonitor = null;
        private readonly ILogger _logger;
        private string serviceName = "Strict Parent Service";
        private string ActualServicePath = "";

        public void RestoreDefaultSettings(Boolean writeToLog)
        {
            if (writeToLog)
                _logger.Information("Restoring Default Registry Settings");

            //open registry and read metadata of installed service

            //service user changed

            Object objectName = GetRegistryKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "ObjectName");

            if ((String)objectName != "LocalSystem")
            {
                _logger.Information($"Restoring Registry Default Settings: Changing {objectName} back to LocalSystem");

                SetRegistryKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "ObjectName", "LocalSystem");
            }

            //service start type

            Object startType = GetRegistryKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "Start");

            if ((Int32)startType != 2)
            {
                _logger.Information($"Restoring Registry Default Settings: Changing {startType} start type back to 2");

                SetRegistryKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "Start", 2);

            }

            //service path changed

            Object modifiedServicePath = GetRegistryKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "ImagePath");

            if ((String)modifiedServicePath != ActualServicePath)
            {
                _logger.Information($"Restoring Registry Default Settings: Changing {objectName} back to LocalSystem");

                SetRegistryKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "ImagePath", ActualServicePath);
            }
        }


        public void StopRegistryMonitor()
        {
            _logger.Information("Stopping Registry Monitor");

            if (RegMonitor == null || RegMonitor.IsMonitoring == false)
            {
                _logger.Information("Registry Monitor is null or already not monitoring");

                return;
            }

            RegMonitor.Stop();

            _logger.Information($"Registry monitor is monitoring = {RegMonitor.IsMonitoring}");
        }

        public void StartRegistryMonitor()
        {
            _logger.Information("Starting Registry Monitor");

            Microsoft.Win32.RegistryKey browserKeys = null;
            try
            {
                browserKeys = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, true);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while starting the registry monitor");
                return;
            }

            if (browserKeys == null)
            {
                _logger.Error($"No key found for service named {serviceName}");
                return;
            }

            Object path = GetRegistryKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName, "ImagePath");

            ActualServicePath = (String)path;

            RegMonitor = new RegistryMonitor(browserKeys);
            RegMonitor.Error += RegMonitor_Error;
            RegMonitor.RegChanged += RegMonitor_RegChanged;
            RegMonitor.RegChangeNotifyFilter = RegChangeNotifyFilter.Value;
            RegMonitor.Start();

            _logger.Information($"Registry monitor is monitoring = {RegMonitor.IsMonitoring}");
        }

        private void RegMonitor_RegChanged(Object sender, EventArgs e)
        {
            _logger.Information("Registry Key Change Detected For Service " + serviceName);

            RestoreDefaultSettings(true);
        }

        private void RegMonitor_Error(Object sender, System.IO.ErrorEventArgs e)
        {
            _logger.Error(e.GetException(), "An error occurred while executing the registry monitor");
        }

        public Object GetRegistryKey(String subKey, String keyName)
        {
            Microsoft.Win32.RegistryKey browserKeys = Microsoft.Win32.Registry
                   .LocalMachine.OpenSubKey(subKey, true);

            if (browserKeys is null)
            {
                _logger.Error($"GetRegistryKey Error: while trying to retrieve a registry key in HKLM: subkey = {subKey}, keyname = {keyName}. key is null");
                return null;
            }

            return browserKeys.GetValue(keyName);
        }


        /// <summary>
        /// Update Or Create a new registry key-value pair
        /// </summary>
        /// <param name="subKey">Ex: SOFTWARE\StrictParent </param>
        /// <param name="keyName">registry key name</param>
        /// <param name="keyValue">registry key value</param>
        public void SetRegistryKey(String subKey, String keyName, Object keyValue)
        {
            Microsoft.Win32.RegistryKey browserKeys = null;
            browserKeys = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(subKey, true);

            if (browserKeys == null)
            {
                _logger.Error($"SetRegistryKey Error: while trying to open a subkey in HKLM: subkey = {subKey}, keyname = {keyName}, keyvalue = {keyValue}. key does not exist.");

                Microsoft.Win32.Registry.LocalMachine.CreateSubKey(subKey, true);

                browserKeys = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(subKey, true);

                browserKeys.SetValue(keyName, keyValue);

                browserKeys.Close();

            }
            else
            {
                browserKeys.SetValue(keyName, keyValue);
                browserKeys.Close();
            }
        }
    }
}