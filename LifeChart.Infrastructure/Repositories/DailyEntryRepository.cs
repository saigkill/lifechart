using LifeChart.Domain.Entries;
using LifeChart.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LifeChart.Infrastructure.Repositories;

public class DailyEntryRepository : IDailyEntryRepository
{
    private readonly LifeChartDbContext _context;

    public DailyEntryRepository(LifeChartDbContext context)
        => _context = context;

    public async Task<DailyEntry?> GetByDateAsync(DateOnly date)
        => await _context.DailyEntries.FirstOrDefaultAsync(e => e.Date == date);

    public async Task<IEnumerable<DailyEntry>> GetRangeAsync(DateOnly from, DateOnly to)
        => await _context.DailyEntries
            .Where(e => e.Date >= from && e.Date <= to)
            .OrderBy(e => e.Date)
            .ToListAsync();

    public async Task<bool> ExistsForTodayAsync()
        => await _context.DailyEntries
            .AnyAsync(e => e.Date == DateOnly.FromDateTime(DateTime.Today));

    public async Task SaveAsync(DailyEntry entry)
    {
        var existing = await GetByDateAsync(entry.Date);
        if (existing is null)
            _context.DailyEntries.Add(entry);
        else
            _context.Entry(existing).CurrentValues.SetValues(entry);

        await _context.SaveChangesAsync();
    }
}
