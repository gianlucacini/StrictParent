using System;

namespace StrictParent.Common
{
    public interface ISettings
    {
        String UnplugFrom { get; set; }
        String UnplugUntil { get; set; }
        String TimeZoneID { get; set; }
        Boolean Unkillable { get; set; }
        DateTime RunUntil { get; set; }
    }
}
