namespace LifeChart.Application.DTOs;

public record CrisisResourceDto(
    string Name,
    string PhoneNumber,
    string? Url,
    bool IsAvailable24h
);
