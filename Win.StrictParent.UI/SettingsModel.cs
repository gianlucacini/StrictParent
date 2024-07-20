using StrictParent.Common;
using System;

namespace StrictParent.UI
{
    internal class SettingsModel : ISettings
    {
        public String UnplugFrom { get; set; }
        public String UnplugUntil { get; set; }
        public String TimeZoneID { get; set; }
        public Boolean Unkillable { get; set; }
        public DateTime RunUntil { get; set; }
    }
}
