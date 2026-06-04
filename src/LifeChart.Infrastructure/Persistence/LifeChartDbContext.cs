using LifeChart.Domain.Entries;
using LifeChart.Domain.Medications;
using Microsoft.EntityFrameworkCore;

namespace LifeChart.Infrastructure.Persistence;

public class LifeChartDbContext : DbContext
{
    public DbSet<DailyEntry> DailyEntries => Set<DailyEntry>();
    public DbSet<Medication> Medications => Set<Medication>();

    public LifeChartDbContext(DbContextOptions<LifeChartDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LifeChartDbContext).Assembly);
    }
}
