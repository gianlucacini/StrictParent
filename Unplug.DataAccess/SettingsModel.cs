using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unplug.Common;

namespace Unplug.DataAccess
{
    class SettingsModel : ISettings
    {
        public String UnplugFrom { get; set; }
        public String UnplugUntil { get; set; }
        public String TimeZoneID { get; set; }
        public Boolean Unkillable { get; set; }
        public DateTime RunUntil { get; set; }
    }
}
