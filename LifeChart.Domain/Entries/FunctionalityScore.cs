namespace LifeChart.Domain.Entries;

public record FunctionalityScore
{
    public int Value { get; }

    public FunctionalityScore(int value)
    {
        if (value < -5 || value > 5)
            throw new ArgumentOutOfRangeException(
                nameof(value), "Funktionsfähigkeit muss zwischen -5 und +5 liegen.");
        Value = value;
    }

    public bool IsCritical => Value <= -4;
    public static FunctionalityScore Neutral => new(0);
}
