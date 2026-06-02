using FluentAssertions;
using LifeChart.Domain.Entries;

namespace LifeChart.Tests.Domain.Entries;

public class SleepHoursTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    [InlineData(24)]
    public void Constructor_ValidValue_SetsValue(int value)
    {
        var hours = new SleepHours(value);
        hours.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(25)]
    public void Constructor_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
    {
        var act = () => new SleepHours(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Zero_ReturnsZero()
    {
        SleepHours.Zero.Value.Should().Be(0);
    }
}
