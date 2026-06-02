using FluentAssertions;
using LifeChart.Domain.Medications;

namespace LifeChart.Tests.Domain.Medications;

public class IntakeTimeTests
{
    [Fact]
    public void Constructor_ValidValues_SetsProperties()
    {
        var time = new TimeOnly(8, 0);
        var intakeTime = new IntakeTime(time, 2);

        intakeTime.Time.Should().Be(time);
        intakeTime.DoseCount.Should().Be(2);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_InvalidDoseCount_ThrowsArgumentOutOfRangeException(int doseCount)
    {
        var act = () => new IntakeTime(new TimeOnly(8, 0), doseCount);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void TwoIntakeTimesWithSameValues_AreEqual()
    {
        var a = new IntakeTime(new TimeOnly(8, 0), 1);
        var b = new IntakeTime(new TimeOnly(8, 0), 1);
        a.Should().Be(b);
    }

    [Fact]
    public void TwoIntakeTimesWithDifferentTime_AreNotEqual()
    {
        var a = new IntakeTime(new TimeOnly(8, 0), 1);
        var b = new IntakeTime(new TimeOnly(20, 0), 1);
        a.Should().NotBe(b);
    }
}
