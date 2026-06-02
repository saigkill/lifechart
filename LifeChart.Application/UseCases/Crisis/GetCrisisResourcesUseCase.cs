using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;

namespace LifeChart.Application.UseCases.Crisis;

public class GetCrisisResourcesUseCase
{
    private readonly ICrisisResourceService _service;

    public GetCrisisResourcesUseCase(ICrisisResourceService service)
        => _service = service;

    public async Task<IEnumerable<CrisisResourceDto>> ExecuteAsync(string regionCode)
        => await _service.GetForRegionAsync(regionCode);
}
