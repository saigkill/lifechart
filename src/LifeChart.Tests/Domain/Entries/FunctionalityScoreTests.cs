using FluentAssertions;
using LifeChart.Domain.Entries;

namespace LifeChart.Tests.Domain.Entries;

public class FunctionalityScoreTests
{
    [Theory]
    [InlineData(-5)]
    [InlineData(0)]
    [InlineData(5)]
    public void Constructor_ValidValue_SetsValue(int value)
    {
        var score = new FunctionalityScore(value);
        score.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-6)]
    [InlineData(6)]
    public void Constructor_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
    {
        var act = () => new FunctionalityScore(value);
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
        var score = new FunctionalityScore(value);
        score.IsCritical.Should().Be(expected);
    }

    [Fact]
    public void Neutral_ReturnsZero()
    {
        FunctionalityScore.Neutral.Value.Should().Be(0);
    }
}
