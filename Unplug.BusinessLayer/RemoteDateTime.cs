using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unplug.BusinessLayer
{
    public class RemoteDateTime 
    {
        static RemoteDateTime()
        {
            StopWatch = new System.Diagnostics.Stopwatch();
        }

        static System.Diagnostics.Stopwatch StopWatch { get; set; }

        static Nullable<DateTime> now;

        /// <summary>
        /// The Actual Date and Time Obtained From The NTP Server. 
        /// can be null if we couldn't retrieve the date and time from the NTP Server.
        /// </summary>
        public static Nullable<DateTime> Now(String timeZoneID)
        {
            if (now is null)
            {
                now = TimeService.GetUtcDateTime();

                if (now is null)
                {
                    //Could not retrieve date and time remotely
                    StopWatch.Stop();

                    now = null;
                }
                else
                {
                    //convert datetime from utc to local
                    DateTime localDt = TimeZoneInfo.ConvertTimeFromUtc(now.Value, TimeZoneInfo.FindSystemTimeZoneById(timeZoneID));

                    StopWatch.Restart();

                    now = new DateTime(localDt.Year, localDt.Month, localDt.Day, localDt.Hour, localDt.Minute, localDt.Second);

                    Log.Information($"Local Datetime of Timezone {timeZoneID} is {now}");

                }
            }
            else
            {
                //calculate time elapsed since last stopwatch start
                long elaps = StopWatch.ElapsedMilliseconds;

                //and add elapsed milliseconds to the last datetime retrieved from the server
                now = now.Value.AddMilliseconds(elaps);

                Log.Information($"Calculated Datetime of Timezone {timeZoneID} is {now}");

                StopWatch.Restart();
            }

            return now;
        }
    }
}
