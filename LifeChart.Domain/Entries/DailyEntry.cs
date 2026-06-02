namespace LifeChart.Domain.Entries;

public class DailyEntry
{
    public int Id { get; private set; }
    public DateOnly Date { get; private set; }
    public MoodScore Mood { get; private set; } = MoodScore.Neutral;
    public FunctionalityScore Functionality { get; private set; } = FunctionalityScore.Neutral;
    public SleepHours SleepHours { get; private set; } = SleepHours.Zero;
    public bool MedicationTaken { get; private set; }
    public bool MenstrualCycle { get; private set; }
    public string? Symptoms { get; private set; }
    public string? Notes { get; private set; }

    public DailyEntry(
        DateOnly date,
        MoodScore mood,
        FunctionalityScore functionality,
        SleepHours sleepHours,
        bool medicationTaken,
        bool menstrualCycle,
        string? symptoms = null,
        string? notes = null)
    {
        Date = date;
        Mood = mood;
        Functionality = functionality;
        SleepHours = sleepHours;
        MedicationTaken = medicationTaken;
        MenstrualCycle = menstrualCycle;
        Symptoms = symptoms;
        Notes = notes;
    }

    public bool IsCritical => Mood.IsCritical && Functionality.IsCritical;

    private DailyEntry() { }
}
