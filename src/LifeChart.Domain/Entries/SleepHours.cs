namespace LifeChart.Domain.Entries;

public record SleepHours
{
    public int Value { get; }

    public SleepHours(int value)
    {
        if (value < 0 || value > 24)
            throw new ArgumentOutOfRangeException(
                nameof(value), "Schlafdauer muss zwischen 0 und 24 Stunden liegen.");
        Value = value;
    }

    public static SleepHours Zero => new(0);
}
