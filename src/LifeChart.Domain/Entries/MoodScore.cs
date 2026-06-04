namespace LifeChart.Domain.Entries;

public record MoodScore
{
    public int Value { get; }

    public MoodScore(int value)
    {
        if (value < -5 || value > 5)
            throw new ArgumentOutOfRangeException(
                nameof(value), "Stimmungswert muss zwischen -5 und +5 liegen.");
        Value = value;
    }

    public bool IsCritical => Value <= -4;
    public static MoodScore Neutral => new(0);
}
