using Serilog;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace StrictParent.Service.Services
{
    /// <summary>
    /// Get the current date time from an ntp server, then updates it using the running stopwatch (when internet is blocked)
    /// CONTEXT:
    /// using the windows local time is not possible, as the user could change date and time, thus bypassing the block.
    /// </summary>
    public class DateTimeService
    {
        public DateTimeService(ILogger logger)
        {
            _logger = logger;
        }
        private readonly ILogger _logger;
        Stopwatch StopWatch { get; set; } = new Stopwatch();

        Nullable<DateTime> now;
        public Nullable<DateTime> Now(String timeZoneID)
        {
            if (now is null)
            {
                now = GetUtcDateTime();

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

                    _logger.Information($"Local Datetime of Timezone {timeZoneID} is {now}");

                }
            }
            else
            {
                //calculate time elapsed since last stopwatch start
                long elaps = StopWatch.ElapsedMilliseconds;

                //and add elapsed milliseconds to the last datetime retrieved from the server
                now = now.Value.AddMilliseconds(elaps);

                _logger.Information($"Calculated Datetime of Timezone {timeZoneID} is {now}");

                StopWatch.Restart();
            }

            return now;
        }

        Nullable<DateTime> GetUtcDateTime()
        {
            _logger.Information("Retrieving UTC DateTime from NTP...");

            DateTime date = new DateTime(1900, 1, 1);

            int tryNum = 0;

            do
            {
                try
                {
                    tryNum++;

                    using (Socket sk = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        sk.ReceiveTimeout = 3000;

                        sk.Connect("time.nist.gov", 123);

                        byte[] data = new byte[] { 0x23, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                        sk.Send(data);

                        sk.Receive(data);

                        byte offTime = 40;

                        byte[] integerPart = new byte[]
                        {
                            data[offTime + 3],
                            data[offTime + 2],
                            data[offTime + 1],
                            data[offTime + 0]
                        };

                        byte[] fractPart = new byte[]
                        {
                            data[offTime + 7],
                            data[offTime + 6],
                            data[offTime + 5],
                            data[offTime + 4]
                        };

                        long ms = (long)(
                              (ulong)BitConverter.ToUInt32(integerPart, 0) * 1000
                             + ((ulong)BitConverter.ToUInt32(fractPart, 0) * 1000)
                              / 0x100000000L);

                        sk.Close();

                        date += TimeSpan.FromTicks(ms * TimeSpan.TicksPerMillisecond);

                        _logger.Information($"UTC DateTime Found. Result = {date}");

                        return date;
                    }
                }
                catch (SocketException se)
                {
                    _logger.Error(se, $"Failed to retreive current datetime from ntp server. Try Number {tryNum}");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Exception while retreiving datetime from ntp server. Try Number {tryNum}");
                }
                finally
                {
                    System.Threading.Thread.Sleep(4000);
                }

            } while (date == new DateTime(1900, 1, 1) && tryNum < 5);

            return null;
        }
    }
}