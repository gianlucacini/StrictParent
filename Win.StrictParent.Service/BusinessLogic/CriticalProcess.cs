using Serilog;
using StrictParent.Common;
using StrictParent.Common.Models;
using System;

namespace StrictParent.Service.BusinessLogic
{
    public class CriticalProcess
    {
        [System.Runtime.InteropServices.DllImport("ntdll.dll", SetLastError = true)]
        private static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);

        public void StatusChanged(StatusResponse sr)
        {
            ISettings s = DataAccess.Settings.LoadSettings();

            if (sr.Interval.HasValue == false)
            {
                SetProcessAsNotCritical(s);
            }
            else
            {
                SetProcessAsCritical(s);
            }
        }
        private Boolean ProcessIsCritical = false;

        private void SetProcessAsCritical(ISettings settings)
        {
            if (ProcessIsCritical == false)
            {
                ProcessIsCritical = true;

                if (settings.Unkillable == false)
                    return;

                Log.Information("PROCESS SET AS CRITICAL");
#if !DEBUG
                System.Diagnostics.Process.EnterDebugMode();
                RtlSetProcessIsCritical(1, 0, 0);
#endif
            }
        }

        public void SetProcessAsNotCritical(ISettings settings)
        {
            if (ProcessIsCritical)
            {
                ProcessIsCritical = false;

                if (settings.Unkillable == false)
                    return;

                Log.Information("PROCESS SET AS NOT CRITICAL");

#if !DEBUG
                RtlSetProcessIsCritical(0, 0, 0);
#endif
            }
        }
    }
}
