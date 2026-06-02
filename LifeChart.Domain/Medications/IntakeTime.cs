namespace LifeChart.Domain.Medications;

public record IntakeTime
{
    public TimeOnly Time { get; private set; }
    public int DoseCount { get; private set; }

    public IntakeTime(TimeOnly time, int doseCount)
    {
        if (doseCount < 1)
            throw new ArgumentOutOfRangeException(
                nameof(doseCount), "Dosisanzahl muss mindestens 1 sein.");
        Time = time;
        DoseCount = doseCount;
    }

    private IntakeTime() { }
}
