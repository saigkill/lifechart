using LifeChart.Application.Interfaces;
using LifeChart.Domain.Entries;
using LifeChart.Domain.Medications;
using LifeChart.Infrastructure.CrisisResources;
using LifeChart.Infrastructure.Persistence;
using LifeChart.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LifeChart.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LifeChart",
            "lifechart.db");

        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        services.AddDbContext<LifeChartDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<IDailyEntryRepository, DailyEntryRepository>();
        services.AddScoped<IMedicationRepository, MedicationRepository>();

        // Crisis resources
        services.AddHttpClient<FindAHelplineService>();
        services.AddSingleton<StaticCrisisResourceService>();
        services.AddScoped<ICrisisResourceService, HybridCrisisResourceService>();

        return services;
    }
}
