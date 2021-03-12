using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Unplug.Common
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
