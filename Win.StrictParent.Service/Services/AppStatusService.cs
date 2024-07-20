using StrictParent.Common;
using StrictParent.Common.Models;
using System;
using System.Threading.Tasks;

namespace StrictParent.Service.Services
{
    public class AppStatusService
    {
        public AppStatusService(DateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
        }

        private readonly DateTimeService _dateTimeService;
        public async Task<StatusResponse> CheckStatus(ISettings settings)
        {
            return await Task.Run(() =>
            {
                StatusResponse sr = new StatusResponse();

                sr.TimeStamp = DateTime.Now;

                DateTime? now = _dateTimeService.Now(settings.TimeZoneID);

                if (now.HasValue == false)
                {
                    //offline, check back in 30 sec
                    sr.Interval = 30000;
                    sr.Status = AppStatus.Unknown;
                    return sr;
                }

                sr.TimeStamp = now.Value;

                if (settings.RunUntil > now.Value)
                {
                    DateTime[] parsedDateTimes = ParseCorrectDateTime(now.Value, settings.UnplugFrom, settings.UnplugUntil);

                    StatusResponse _sResponse = CalculateStatusInterval(now.Value, parsedDateTimes[0], parsedDateTimes[1]);

                    sr.Status = _sResponse.Status;
                    sr.Interval = _sResponse.Interval;

                }
                else
                {
                    sr.Status = AppStatus.Online;
                    sr.Interval = null;
                }

                return sr;
            });
        }

        public DateTime[] ParseCorrectDateTime(DateTime now, String fromTimeStr, String untilTimeStr)
        {
            DateTime[] dateTimes = new DateTime[2];

            DateTime from = DateTime.Parse(fromTimeStr);

            DateTime until = DateTime.Parse(untilTimeStr);

            from = new DateTime(now.Year, now.Month, now.Day, from.Hour, from.Minute, 0);

            until = new DateTime(now.Year, now.Month, now.Day, until.Hour, until.Minute, 0);

            if (from.Hour * 60 + from.Minute > until.Hour * 60 + until.Minute)
            {
                if (now.Hour * 60 + now.Minute > until.Hour * 60 + until.Minute)
                {
                    //begin block is today
                    dateTimes[0] = new DateTime(now.Year, now.Month, now.Day, from.Hour, from.Minute, 0);
                    //end block is tomorrow
                    dateTimes[1] = new DateTime(now.Year, now.Month, now.Day, until.Hour, until.Minute, 0).AddDays(1);
                }
                else
                {
                    //begin block was yesterday
                    dateTimes[0] = new DateTime(now.Year, now.Month, now.Day, from.Hour, from.Minute, 0).AddDays(-1);

                    //end block is today
                    dateTimes[1] = new DateTime(now.Year, now.Month, now.Day, until.Hour, until.Minute, 0);

                }
            }
            else
            {
                //both begin and end block are today
                dateTimes[0] = new DateTime(now.Year, now.Month, now.Day, from.Hour, from.Minute, 0);
                dateTimes[1] = new DateTime(now.Year, now.Month, now.Day, until.Hour, until.Minute, 0);

            }

            return dateTimes;
        }

        public StatusResponse CalculateStatusInterval(DateTime now, DateTime unplugFrom, DateTime unplugUntil)
        {

            if (unplugFrom <= now && now <= unplugUntil)
            {
                //we are offline

                return new StatusResponse()
                {
                    Interval = ToSafeInterval((unplugUntil - now).TotalMilliseconds),
                    Status = AppStatus.Offline
                };
            }
            else
            {
                //we are online
                if (now < unplugFrom)
                {
                    return new StatusResponse()
                    {
                        Interval = ToSafeInterval((unplugFrom - now).TotalMilliseconds),
                        Status = AppStatus.Online
                    };
                }
                else
                {
                    //now > unplugUntil

                    return new StatusResponse()
                    {
                        Interval = ToSafeInterval((unplugFrom.AddDays(1) - now).TotalMilliseconds),
                        Status = AppStatus.Online
                    };
                }
            }








            //if (unplugFrom.Hour * 60 + unplugFrom.Minute > unplugUntil.Hour * 60 + unplugUntil.Minute)
            //{
            //    if (now.Hour * 60 + now.Minute > unplugUntil.Hour * 60 + unplugUntil.Minute)
            //    {
            //        //block begins today and ends tomorrow, so we are still online

            //        DateTime nextBlock = new DateTime(now.Year, now.Month, now.Day, unplugFrom.Hour, unplugFrom.Minute, 0);

            //        TimeSpan ts = nextBlock - now;

            //        return new StatusResponse()
            //        {
            //            Interval = ToSafeInterval(ts.TotalMilliseconds),
            //            Status = AppStatus.Online
            //        };

            //    }
            //    else
            //    {
            //        //block begun yesterday and is going to end today, so we are offline

            //        DateTime endBlock = new DateTime(now.Year, now.Month, now.Day, unplugUntil.Hour, unplugUntil.Minute, 0);

            //        TimeSpan ts = endBlock - now;

            //        return new StatusResponse()
            //        {
            //            Interval = ToSafeInterval(ts.TotalMilliseconds),
            //            Status = AppStatus.Offline
            //        };
            //    }
            //}
            //else
            //{
            //    //block begins today and ends today, so we are still online

            //    DateTime nextBlock = new DateTime(now.Year, now.Month, now.Day, unplugFrom.Hour, unplugFrom.Minute, 0);

            //    TimeSpan ts = nextBlock - now;

            //    return new StatusResponse()
            //    {
            //        Interval = ToSafeInterval(ts.TotalMilliseconds),
            //        Status = AppStatus.Online
            //    };
            //}

        }
        private Double ToSafeInterval(Double calculatedMilliseconds)
        {
            //TODO test, timer does not accept values < 100 milliseconds

            if (calculatedMilliseconds < 100)
                return 100;

            if (calculatedMilliseconds > Int32.MaxValue)
                return Int32.MaxValue;
            else
                return calculatedMilliseconds;
        }
    }
}