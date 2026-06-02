namespace LifeChart.Application.DTOs;

public record IntakeTimeDto(string Time, int DoseCount);

public record MedicationDto(
    int Id,
    string Name,
    string Dosage,
    int MinStock,
    int CurrentStock,
    bool IsStockLow,
    IReadOnlyList<IntakeTimeDto> IntakeTimes
);
