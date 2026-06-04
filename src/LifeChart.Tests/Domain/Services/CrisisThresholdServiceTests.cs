using FluentAssertions;
using LifeChart.Domain.Entries;
using LifeChart.Domain.Services;

namespace LifeChart.Tests.Domain.Services;

public class CrisisThresholdServiceTests
{
    private readonly CrisisThresholdService _service = new();

    private static DailyEntry CreateEntry(int mood, int functionality) => new(
        DateOnly.FromDateTime(DateTime.Today),
        new MoodScore(mood),
        new FunctionalityScore(functionality),
        new SleepHours(7),
        medicationTaken: false,
        menstrualCycle: false);

    [Fact]
    public void IsCritical_BothScoresCritical_ReturnsTrue()
    {
        var entry = CreateEntry(-5, -5);
        _service.IsCritical(entry).Should().BeTrue();
    }

    [Fact]
    public void IsCritical_OnlyMoodCritical_ReturnsFalse()
    {
        var entry = CreateEntry(-5, 0);
        _service.IsCritical(entry).Should().BeFalse();
    }

    [Fact]
    public void IsCritical_OnlyFunctionalityCritical_ReturnsFalse()
    {
        var entry = CreateEntry(0, -5);
        _service.IsCritical(entry).Should().BeFalse();
    }

    [Fact]
    public void IsCritical_NeitherCritical_ReturnsFalse()
    {
        var entry = CreateEntry(0, 0);
        _service.IsCritical(entry).Should().BeFalse();
    }

    [Theory]
    [InlineData(-4, -4)]
    [InlineData(-5, -4)]
    [InlineData(-4, -5)]
    public void IsCritical_AtThreshold_ReturnsTrue(int mood, int func)
    {
        var entry = CreateEntry(mood, func);
        _service.IsCritical(entry).Should().BeTrue();
    }

    [Theory]
    [InlineData(-3, -3)]
    [InlineData(-3, -4)]
    [InlineData(-4, -3)]
    public void IsCritical_JustBelowThreshold_ReturnsFalse(int mood, int func)
    {
        var entry = CreateEntry(mood, func);
        _service.IsCritical(entry).Should().BeFalse();
    }
}
