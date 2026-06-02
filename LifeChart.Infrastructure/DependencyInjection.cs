using LifeChart.Application.Interfaces;
using LifeChart.Application.Settings;
using LifeChart.Domain.Entries;
using LifeChart.Domain.Medications;
using LifeChart.Infrastructure.Backup;
using LifeChart.Infrastructure.CrisisResources;
using LifeChart.Infrastructure.Pdf;
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

        // PDF
        services.AddScoped<IPdfRenderer, PdfSharpRenderer>();

        // Backup providers
        services.AddSingleton<LocalExportProvider>();
        services.AddHttpClient<NextcloudBackupProvider>();
#if GOOGLE_SERVICES
        services.AddHttpClient<GoogleDriveBackupProvider>();
#endif
        services.AddScoped<IBackupProvider>(sp =>
        {
            var settings = sp.GetRequiredService<ISettingsService>().Load();
            return settings.BackupProvider switch
            {
#if GOOGLE_SERVICES
                CloudProvider.GoogleDrive =>
                    sp.GetRequiredService<GoogleDriveBackupProvider>(),
#endif
                CloudProvider.Nextcloud =>
                    sp.GetRequiredService<NextcloudBackupProvider>(),
                _ => sp.GetRequiredService<LocalExportProvider>()
            };
        });

        // Crisis resources
        services.AddHttpClient<FindAHelplineService>();
        services.AddSingleton<StaticCrisisResourceService>();
        services.AddScoped<ICrisisResourceService, HybridCrisisResourceService>();

        return services;
    }
}
