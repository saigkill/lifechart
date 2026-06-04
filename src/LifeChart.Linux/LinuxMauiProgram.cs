using System.Globalization;
using LifeChart.Application;
using LifeChart.Application.Interfaces;
using LifeChart.Application.Localization;
using LifeChart.Application.Settings;
using LifeChart.Infrastructure;
using LifeChart.Infrastructure.Persistence;
using LifeChart.Linux.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platforms.Linux.Gtk4.Hosting;

namespace LifeChart;

public static class LinuxMauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp
            .CreateBuilder()
            .UseMauiAppLinuxGtk4<LinuxApp>();

        builder.Services
            .AddApplication()
            .AddInfrastructure();

        builder.Services.AddSingleton<ISettingsService, LinuxSettingsService>();
        builder.Services.AddSingleton<IBiometricService, NullBiometricService>();
        builder.Services.AddSingleton<IAlarmService, LinuxAlarmService>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LifeChartDbContext>();
            db.Database.Migrate();

            var settings = scope.ServiceProvider.GetRequiredService<ISettingsService>().Load();
            L.SetCulture(settings.Language switch
            {
                AppLanguage.English => new CultureInfo("en"),
                AppLanguage.German  => new CultureInfo("de"),
                _                   => CultureInfo.CurrentUICulture,
            });
        }

        return app;
    }
}
