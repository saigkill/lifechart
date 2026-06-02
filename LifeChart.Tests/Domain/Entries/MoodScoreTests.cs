using FluentAssertions;
using LifeChart.Domain.Entries;

namespace LifeChart.Tests.Domain.Entries;

public class MoodScoreTests
{
    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public void Constructor_ValidValue_SetsValue(int value)
    {
        var score = new MoodScore(value);
        score.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-6)]
    [InlineData(6)]
    [InlineData(-100)]
    [InlineData(100)]
    public void Constructor_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
    {
        var act = () => new MoodScore(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-5, true)]
    [InlineData(-4, true)]
    [InlineData(-3, false)]
    [InlineData(0, false)]
    [InlineData(5, false)]
    public void IsCritical_ReturnsCorrectValue(int value, bool expected)
    {
        var score = new MoodScore(value);
        score.IsCritical.Should().Be(expected);
    }

    [Fact]
    public void Neutral_ReturnsZero()
    {
        MoodScore.Neutral.Value.Should().Be(0);
    }

    [Fact]
    public void TwoScoresWithSameValue_AreEqual()
    {
        var a = new MoodScore(3);
        var b = new MoodScore(3);
        a.Should().Be(b);
    }

    [Fact]
    public void TwoScoresWithDifferentValues_AreNotEqual()
    {
        var a = new MoodScore(3);
        var b = new MoodScore(-3);
        a.Should().NotBe(b);
    }
}
