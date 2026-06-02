using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;

namespace LifeChart.Infrastructure.CrisisResources;

public class HybridCrisisResourceService : ICrisisResourceService
{
    private readonly FindAHelplineService _apiService;
    private readonly StaticCrisisResourceService _staticService;

    public HybridCrisisResourceService(
        FindAHelplineService apiService,
        StaticCrisisResourceService staticService)
    {
        _apiService = apiService;
        _staticService = staticService;
    }

    public async Task<IEnumerable<CrisisResourceDto>> GetForRegionAsync(string regionCode)
    {
        try
        {
            var results = (await _apiService.GetForRegionAsync(regionCode)).ToList();
            if (results.Count > 0)
                return results;
        }
        catch
        {
            // API nicht erreichbar — Fallback auf statische Liste
        }

        return await _staticService.GetForRegionAsync(regionCode);
    }
}
