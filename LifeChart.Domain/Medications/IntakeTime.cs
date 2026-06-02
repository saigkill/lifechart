namespace LifeChart.Domain.Medications;

public record IntakeTime
{
    public TimeOnly Time { get; }
    public int DoseCount { get; }

    public IntakeTime(TimeOnly time, int doseCount)
    {
        if (doseCount < 1)
            throw new ArgumentOutOfRangeException(
                nameof(doseCount), "Dosisanzahl muss mindestens 1 sein.");
        Time = time;
        DoseCount = doseCount;
    }
}
