namespace LifeChart.Domain.Entries;

public interface IDailyEntryRepository
{
    Task<DailyEntry?> GetByDateAsync(DateOnly date);
    Task<IEnumerable<DailyEntry>> GetRangeAsync(DateOnly from, DateOnly to);
    Task<bool> ExistsForTodayAsync();
    Task SaveAsync(DailyEntry entry);
}
