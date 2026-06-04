using LifeChart.Application.DTOs;
using LifeChart.Domain.Entries;

namespace LifeChart.Application.UseCases.Entries;

public class GetTodayEntryUseCase
{
    private readonly IDailyEntryRepository _repository;

    public GetTodayEntryUseCase(IDailyEntryRepository repository)
        => _repository = repository;

    public async Task<DailyEntryDto?> ExecuteAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var entry = await _repository.GetByDateAsync(today);
        if (entry is null) return null;

        return new DailyEntryDto(
            entry.Date,
            entry.Mood.Value,
            entry.Functionality.Value,
            entry.SleepHours.Value,
            entry.MedicationTaken,
            entry.MenstrualCycle,
            entry.Symptoms,
            entry.Notes,
            entry.IsHypomanic);
    }
}
