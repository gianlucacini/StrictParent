using System;

namespace StrictParent.Common.Models
{
    public class StatusResponse
    {
        public AppStatus Status { get; set; } = AppStatus.Unknown;
        public Double? Interval { get; set; } = 30 * 1000;
        public DateTime TimeStamp { get; set; }
        public override String ToString() =>
            $"Status => '{this.Status}' / Interval => '{Interval}' / TimeStamp => '{TimeStamp}'";
    }
}
