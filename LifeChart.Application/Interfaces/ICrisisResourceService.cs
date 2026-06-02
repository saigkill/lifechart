using LifeChart.Application.DTOs;

namespace LifeChart.Application.Interfaces;

public interface ICrisisResourceService
{
    Task<IEnumerable<CrisisResourceDto>> GetForRegionAsync(string regionCode);
}
