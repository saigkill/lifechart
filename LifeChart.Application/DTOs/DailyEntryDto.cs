namespace LifeChart.Application.DTOs;

public record DailyEntryDto(
    DateOnly Date,
    int Mood,
    int Functionality,
    int SleepHours,
    bool MedicationTaken,
    bool MenstrualCycle,
    string? Symptoms,
    string? Notes
);
