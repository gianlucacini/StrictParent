using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Unplug.BusinessLayer
{
    public static class TimeService
    {
        public static Nullable<DateTime> GetUtcDateTime()
        {
            Log.Information("Retrieving UTC DateTime from NTP...");

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

                        Log.Information($"UTC DateTime Found. Result = {date}");

                        return date;
                    }
                }
                catch (SocketException se)
                {
                    Log.Error(se, $"Failed to retreive current datetime from ntp server. Try Number {tryNum}");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Exception while retreiving datetime from ntp server. Try Number {tryNum}");
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
