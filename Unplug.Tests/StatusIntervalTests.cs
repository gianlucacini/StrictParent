using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unplug.BusinessLayer;

namespace Unplug.Tests
{
    [TestClass]
    public class StatusIntervalTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            //-- Act

            var actual1 = AppStatusService.CalculateStatusInterval(
                new DateTime(2021, 2, 27, 0, 0, 0),
                new DateTime(2021, 2, 26, 10, 0, 0),
                new DateTime(2021, 2, 27, 6, 0, 0));

            var actual2 = AppStatusService.CalculateStatusInterval(
               new DateTime(2021, 2, 27, 1, 0, 0),
               new DateTime(2021, 2, 27, 0, 0, 0),
               new DateTime(2021, 2, 27, 6, 0, 0));

            var actual3 = AppStatusService.CalculateStatusInterval(
                new DateTime(2021, 2, 27, 6, 0, 0),
                new DateTime(2021, 2, 27, 0, 0, 0),
                new DateTime(2021, 2, 27, 6, 0, 0));

            var actual4 = AppStatusService.CalculateStatusInterval(
                new DateTime(2021, 2, 27, 15, 0, 0),
                new DateTime(2021, 2, 27, 16, 0, 0),
                new DateTime(2021, 2, 27, 17, 0, 0));

            var actual5 = AppStatusService.CalculateStatusInterval(
                new DateTime(2021, 2, 27, 15, 0, 0),
                new DateTime(2021, 2, 27, 10, 0, 0),
                new DateTime(2021, 2, 28, 3, 0, 0));

            var actual6 = AppStatusService.CalculateStatusInterval(
                new DateTime(2021, 2, 27, 15, 0, 0),
                new DateTime(2021, 2, 27, 10, 0, 0),
                new DateTime(2021, 2, 27, 13, 0, 0));

            var actualNowInBetween1 = AppStatusService.CalculateStatusInterval(
                new DateTime(2021, 2, 27, 2, 0, 0),
                new DateTime(2021, 2, 26, 22, 0, 0),
                new DateTime(2021, 2, 27, 4, 0, 0));

            var actualNowInBetween2 = AppStatusService.CalculateStatusInterval(
                new DateTime(2021, 2, 27, 2, 0, 0),
                new DateTime(2021, 2, 27, 1, 0, 0),
                new DateTime(2021, 2, 27, 4, 0, 0));

            var actualNowInBetween3 = AppStatusService.CalculateStatusInterval(
              new DateTime(2021, 2, 27, 23, 30, 0),
              new DateTime(2021, 2, 27, 23, 0, 0),
              new DateTime(2021, 2, 28, 1, 0, 0));


            //-- Assert
            Assert.IsTrue(actual1.Status == AppStatus.Offline);
            Assert.AreEqual(21600000, actual1.Interval.Value);

            Assert.IsTrue(actual2.Status == AppStatus.Offline);
            Assert.AreEqual(18000000, actual2.Interval.Value);

            Assert.IsTrue(actual3.Status == AppStatus.Offline);
            Assert.AreEqual(100, actual3.Interval.Value);

            Assert.IsTrue(actual4.Status == AppStatus.Online);
            Assert.AreEqual(3600000, actual4.Interval.Value);

            Assert.IsTrue(actual5.Status == AppStatus.Offline);
            Assert.AreEqual(43200000, actual5.Interval.Value);

            Assert.IsTrue(actual6.Status == AppStatus.Online);
            Assert.AreEqual(68400000, actual6.Interval.Value);

            Assert.IsTrue(actualNowInBetween1.Status == AppStatus.Offline);
            Assert.IsTrue(actualNowInBetween2.Status == AppStatus.Offline);
            Assert.IsTrue(actualNowInBetween3.Status == AppStatus.Offline);

            Assert.AreEqual(7200000, actualNowInBetween1.Interval.Value);
            Assert.AreEqual(7200000, actualNowInBetween2.Interval.Value);

            Assert.AreEqual(5400000, actualNowInBetween3.Interval.Value);


        }

    }
}
