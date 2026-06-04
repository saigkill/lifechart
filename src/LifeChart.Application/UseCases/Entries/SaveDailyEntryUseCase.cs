using LifeChart.Application.DTOs;
using LifeChart.Domain.Entries;
using LifeChart.Domain.Services;

namespace LifeChart.Application.UseCases.Entries;

public class SaveDailyEntryUseCase
{
    private readonly IDailyEntryRepository _repository;
    private readonly CrisisThresholdService _crisisService;

    public SaveDailyEntryUseCase(
        IDailyEntryRepository repository,
        CrisisThresholdService crisisService)
    {
        _repository = repository;
        _crisisService = crisisService;
    }

    public record Result(bool IsCritical);

    public async Task<Result> ExecuteAsync(DailyEntryDto dto)
    {
        var entry = new DailyEntry(
            dto.Date,
            new MoodScore(dto.Mood),
            new FunctionalityScore(dto.Functionality),
            new SleepHours(dto.SleepHours),
            dto.MedicationTaken,
            dto.MenstrualCycle,
            dto.Symptoms,
            dto.Notes,
            dto.IsHypomanic);

        await _repository.SaveAsync(entry);
        return new Result(_crisisService.IsCritical(entry));
    }
}
