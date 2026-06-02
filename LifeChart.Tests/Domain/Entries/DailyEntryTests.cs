using FluentAssertions;
using LifeChart.Domain.Entries;

namespace LifeChart.Tests.Domain.Entries;

public class DailyEntryTests
{
    private static DailyEntry CreateEntry(int mood, int functionality) => new(
        DateOnly.FromDateTime(DateTime.Today),
        new MoodScore(mood),
        new FunctionalityScore(functionality),
        new SleepHours(7),
        medicationTaken: true,
        menstrualCycle: false);

    [Theory]
    [InlineData(-5, -5, true)]
    [InlineData(-4, -4, true)]
    [InlineData(-4, -5, true)]
    [InlineData(-5, -4, true)]
    public void IsCritical_BothScoresCritical_ReturnsTrue(int mood, int func, bool expected)
    {
        var entry = CreateEntry(mood, func);
        entry.IsCritical.Should().Be(expected);
    }

    [Theory]
    [InlineData(-4, -3)]
    [InlineData(-3, -4)]
    [InlineData(0, 0)]
    [InlineData(-3, -3)]
    [InlineData(5, 5)]
    public void IsCritical_OnlyOneOrNeitherCritical_ReturnsFalse(int mood, int func)
    {
        var entry = CreateEntry(mood, func);
        entry.IsCritical.Should().BeFalse();
    }

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var date = new DateOnly(2026, 6, 1);
        var entry = new DailyEntry(
            date,
            new MoodScore(-2),
            new FunctionalityScore(1),
            new SleepHours(6),
            medicationTaken: true,
            menstrualCycle: true,
            symptoms: "Kopfschmerzen",
            notes: "Schlechter Tag");

        entry.Date.Should().Be(date);
        entry.Mood.Value.Should().Be(-2);
        entry.Functionality.Value.Should().Be(1);
        entry.SleepHours.Value.Should().Be(6);
        entry.MedicationTaken.Should().BeTrue();
        entry.MenstrualCycle.Should().BeTrue();
        entry.Symptoms.Should().Be("Kopfschmerzen");
        entry.Notes.Should().Be("Schlechter Tag");
    }
}
