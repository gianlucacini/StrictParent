using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Unplug.Common
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
