using Libary;

[TestFixture]
public class AdditionalTests
{
    private Calculations calculations;

    [SetUp]
    public void Setup()
    {
        calculations = new Calculations();
    }

    [Test]
    public void TestPeriodEndsExactlyAtWorkEnd()
    {
        var result = calculations.AvailablePeriods(
            new TimeSpan[] { new TimeSpan(16, 0, 0) },
            new int[] { 60 },
            new TimeSpan(9, 0, 0),
            new TimeSpan(17, 0, 0),
            30);

        Assert.AreEqual("09:00-09:30", result[0]);
        Assert.AreEqual("15:30-16:00", result[result.Length - 1]);
    }

    [Test]
    public void TestPeriodStartsExactlyAtWorkBegin()
    {
        var result = calculations.AvailablePeriods(
            new TimeSpan[] { new TimeSpan(9, 0, 0) },
            new int[] { 60 },
            new TimeSpan(9, 0, 0),
            new TimeSpan(17, 0, 0),
            30);

        Assert.AreEqual("10:00-10:30", result[0]);
        Assert.AreEqual("16:30-17:00", result[result.Length - 1]);
    }

    [Test]
    public void TestNotEnoughTimeForConsultation()
    {
        var result = calculations.AvailablePeriods(
            new TimeSpan[] { new TimeSpan(10, 0, 0), new TimeSpan(10, 45, 0) },
            new int[] { 30, 15 },
            new TimeSpan(9, 0, 0),
            new TimeSpan(11, 0, 0),
            30);

       
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("09:00-09:30", result[0]);
        Assert.AreEqual("09:30-10:00", result[1]);
    }

    [Test]
    public void TestConsultationLongerThanFreeSlot()
    {
        var result = calculations.AvailablePeriods(
            new TimeSpan[] { new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0) },
            new int[] { 30, 30 },
            new TimeSpan(9, 0, 0),
            new TimeSpan(12, 0, 0),
            45);

        // Only 09:00-09:45 and 09:45-10:30 fit
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual("09:00-09:45", result[0]);
    }

    [Test]
    public void TestBusyPeriodsOverlap()
    {
        var result = calculations.AvailablePeriods(
            new TimeSpan[] { new TimeSpan(10, 0, 0), new TimeSpan(10, 30, 0) },
            new int[] { 60, 60 },
            new TimeSpan(9, 0, 0),
            new TimeSpan(12, 0, 0),
            30);

        Assert.AreEqual(new[]
        {
            "09:00-09:30",
            "09:30-10:00",
            "11:30-12:00"
        }, result);
    }

    [Test]
    public void TestBusyEndsAtWorkEnd()
    {
        var result = calculations.AvailablePeriods(
            new TimeSpan[] { new TimeSpan(16, 30, 0) },
            new int[] { 30 },
            new TimeSpan(9, 0, 0),
            new TimeSpan(17, 0, 0),
            30);

        Assert.IsFalse(Array.Exists(result, r => r.StartsWith("16:30")));
    }

    [Test]
    public void TestMultipleGapsBetweenBusy()
    {
        var result = calculations.AvailablePeriods(
            new TimeSpan[]
            {
                new TimeSpan(10, 0, 0),
                new TimeSpan(12, 0, 0),
                new TimeSpan(14, 0, 0)
            },
            new int[] { 30, 30, 30 },
            new TimeSpan(9, 0, 0),
            new TimeSpan(15, 0, 0),
            30);

        Assert.AreEqual(new[]
        {
            "09:00-09:30", "09:30-10:00",
            "10:30-11:00", "11:00-11:30", "11:30-12:00",
            "12:30-13:00", "13:00-13:30", "13:30-14:00",
            "14:30-15:00"
        }, result);
    }

    [Test]
    public void TestExactSlotAtEndOfDay()
    {
        var result = calculations.AvailablePeriods(
            Array.Empty<TimeSpan>(),
            Array.Empty<int>(),
            new TimeSpan(16, 30, 0),
            new TimeSpan(17, 0, 0),
            30);

        Assert.AreEqual(1, result.Length);
        Assert.AreEqual("16:30-17:00", result[0]);
    }

    [Test]
    public void TestBusyCoversWholeDay()
    {
        var result = calculations.AvailablePeriods(
            new TimeSpan[] { new TimeSpan(9, 0, 0) },
            new int[] { 480 },
            new TimeSpan(9, 0, 0),
            new TimeSpan(17, 0, 0),
            30);

        Assert.AreEqual(0, result.Length);
    }

    [Test]
    public void TestFreeSlotBeforeFirstBusy()
    {
        var result = calculations.AvailablePeriods(
            new TimeSpan[] { new TimeSpan(9, 30, 0) },
            new int[] { 60 },
            new TimeSpan(9, 0, 0),
            new TimeSpan(11, 0, 0),
            30);

        Assert.AreEqual(new[] { "09:00-09:30", "10:30-11:00" }, result);
    }
}
