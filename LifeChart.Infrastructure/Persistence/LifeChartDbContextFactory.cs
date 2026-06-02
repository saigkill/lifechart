using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LifeChart.Infrastructure.Persistence;

public class LifeChartDbContextFactory : IDesignTimeDbContextFactory<LifeChartDbContext>
{
    public LifeChartDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<LifeChartDbContext>()
            .UseSqlite("Data Source=lifechart_designtime.db")
            .Options;

        return new LifeChartDbContext(options);
    }
}
