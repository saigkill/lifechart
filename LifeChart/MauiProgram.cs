using LifeChart.Application;
using LifeChart.Application.Settings;
using LifeChart.Infrastructure;
using LifeChart.Pages;
using LifeChart.Services;
using LifeChart.ViewModels;
using Microsoft.Extensions.Logging;

namespace LifeChart;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMaui()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Application + Infrastructure layers
        builder.Services
            .AddApplication()
            .AddInfrastructure();

        // Settings
        builder.Services.AddSingleton<ISettingsService, MauiPreferencesSettingsService>();

        // ViewModels
        builder.Services.AddTransient<TodayViewModel>();
        builder.Services.AddTransient<ChartViewModel>();
        builder.Services.AddTransient<MedicationsViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<CrisisViewModel>();
        builder.Services.AddTransient<OnboardingViewModel>();

        // Pages
        builder.Services.AddTransient<TodayPage>();
        builder.Services.AddTransient<ChartPage>();
        builder.Services.AddTransient<MedicationsPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<CrisisPage>();
        builder.Services.AddTransient<OnboardingPage>();
        builder.Services.AddTransient<AppShell>();
        builder.Services.AddSingleton<App>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
