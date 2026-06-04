using LifeChart.Domain.Entries;

namespace LifeChart.Domain.Services;

public class CrisisThresholdService
{
    public bool IsCritical(DailyEntry entry) => entry.IsCritical;
}
