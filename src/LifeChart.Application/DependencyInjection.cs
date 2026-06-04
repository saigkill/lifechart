using LifeChart.Application.UseCases.Backup;
using LifeChart.Application.UseCases.Crisis;
using LifeChart.Application.UseCases.Entries;
using LifeChart.Application.UseCases.Medications;
using LifeChart.Application.UseCases.Pdf;
using LifeChart.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LifeChart.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CrisisThresholdService>();

        services.AddScoped<SaveDailyEntryUseCase>();
        services.AddScoped<GetTodayEntryUseCase>();
        services.AddScoped<GetChartDataUseCase>();

        services.AddScoped<GetActiveMedicationsUseCase>();
        services.AddScoped<SaveMedicationUseCase>();
        services.AddScoped<DeactivateMedicationUseCase>();

        services.AddScoped<ExportPdfUseCase>();

        services.AddScoped<CreateBackupUseCase>();
        services.AddScoped<RestoreBackupUseCase>();

        services.AddScoped<GetCrisisResourcesUseCase>();

        return services;
    }
}
