using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unplug.BusinessLayer
{
    public class StatusResponse
    {
        public StatusResponse()
        {
            this.Status = AppStatus.Unknown;
            this.Interval = 30 * 1000;

        }
        public AppStatus Status { get; set; }
        public Double? Interval { get; set; }
        public DateTime TimeStamp { get; set; }

        public override String ToString()
        {
            String _status = this.Status.ToString();

            return $"Status => '{_status}' / Interval => '{Interval}' / TimeStamp => '{TimeStamp}'";
        }

    }
}
