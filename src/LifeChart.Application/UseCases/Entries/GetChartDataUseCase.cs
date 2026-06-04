using LifeChart.Application.DTOs;
using LifeChart.Domain.Entries;

namespace LifeChart.Application.UseCases.Entries;

public class GetChartDataUseCase
{
    private readonly IDailyEntryRepository _repository;

    public GetChartDataUseCase(IDailyEntryRepository repository)
        => _repository = repository;

    public async Task<ChartDataDto> ExecuteAsync(DateOnly from, DateOnly to)
    {
        var entries = await _repository.GetRangeAsync(from, to);

        var points = entries
            .Select(e => new ChartPointDto(e.Date, e.Mood.Value, e.Functionality.Value, e.IsHypomanic))
            .ToList()
            .AsReadOnly();

        return new ChartDataDto(points, from, to);
    }
}
