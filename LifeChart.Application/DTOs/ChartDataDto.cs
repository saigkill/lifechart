namespace LifeChart.Application.DTOs;

public record ChartPointDto(DateOnly Date, int Mood, int Functionality);

public record ChartDataDto(
    IReadOnlyList<ChartPointDto> Points,
    DateOnly From,
    DateOnly To
);
