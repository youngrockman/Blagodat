using NUnit.Framework;
using System;
using Libary;

namespace Testiks
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestNoBusyPeriods()
        {
            Calculations calculations = new Calculations();
            TimeSpan[] startTimes = Array.Empty<TimeSpan>();
            int[] durations = Array.Empty<int>();
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30;


            string[] result = calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime,
                consultationTime);


            Assert.AreEqual(16, result.Length);
            Assert.AreEqual("09:00-09:30", result[0]);
            Assert.AreEqual("16:30-17:00", result[15]);
        }

        [Test]
        public void TestSingleBusyPeriod()
        {

            Calculations calculations = new Calculations();
            TimeSpan[] startTimes = new TimeSpan[] { new TimeSpan(10, 0, 0) };
            int[] durations = new int[] { 60 };
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30;


            string[] result = calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime,
                consultationTime);


            Assert.AreEqual(14, result.Length);
            Assert.AreEqual("09:00-09:30", result[0]);
            Assert.AreEqual("09:30-10:00", result[1]);
            Assert.AreEqual("11:00-11:30", result[2]);
            Assert.AreEqual("16:30-17:00", result[13]);
        }

        [Test]
        public void TestMultipleBusyPeriods()
        {
            // Arrange
            Calculations calculations = new Calculations();
            TimeSpan[] startTimes = new TimeSpan[]
                { new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0), new TimeSpan(14, 0, 0) };
            int[] durations = new int[] { 60, 30, 90 };
            TimeSpan beginWorkingTime = new TimeSpan(9, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(17, 0, 0);
            int consultationTime = 30;

            // Act
            string[] result = calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime,
                consultationTime);

            // Assert
            Assert.AreEqual(10, result.Length);
            Assert.AreEqual("09:00-09:30", result[0]);
            Assert.AreEqual("09:30-10:00", result[1]);
            Assert.AreEqual("11:00-11:30", result[2]);
            Assert.AreEqual("11:30-12:00", result[3]);
            Assert.AreEqual("12:30-13:00", result[4]);
            Assert.AreEqual("15:30-16:00", result[5]);
            Assert.AreEqual("16:00-16:30", result[6]);
            Assert.AreEqual("16:30-17:00", result[7]);
        }
    }
}
