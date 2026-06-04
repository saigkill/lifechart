namespace LifeChart.Application.DTOs;

public record SaveMedicationDto(
    int Id,
    string Name,
    string Dosage,
    int MinStock,
    int CurrentStock,
    IReadOnlyList<IntakeTimeDto> IntakeTimes
);
