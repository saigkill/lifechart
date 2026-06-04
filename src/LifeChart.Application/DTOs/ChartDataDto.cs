namespace LifeChart.Application.DTOs;

public record ChartPointDto(DateOnly Date, int Mood, int Functionality, bool IsHypomanic = false);

public record ChartDataDto(
    IReadOnlyList<ChartPointDto> Points,
    DateOnly From,
    DateOnly To
);
