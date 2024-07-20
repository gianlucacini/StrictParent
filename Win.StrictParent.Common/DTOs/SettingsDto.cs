using System;
using System.Runtime.Serialization;

namespace StrictParent.Common.DTOs
{
    [DataContract]
    public class SettingsDto : ISettings
    {
        [DataMember]
        public String UnplugFrom { get; set; }

        [DataMember]
        public String UnplugUntil { get; set; }

        [DataMember]
        public String TimeZoneID { get; set; }

        [DataMember]
        public Boolean Unkillable { get; set; }

        [DataMember]
        public DateTime RunUntil { get; set; }

        public override String ToString()
        {
            return $"From = '{this.UnplugFrom}', " +
                $"Until = '{this.UnplugUntil}', " +
                $"TimeZone = '{this.TimeZoneID}', " +
                $"Unkillable = '{this.Unkillable}', " +
                $"RunUntil = '{this.RunUntil}'";
        }

    }
}
