using LifeChart.Application.Settings;
using LifeChart.Pages;
using Microsoft.Extensions.DependencyInjection;
using MauiApplication = Microsoft.Maui.Controls.Application;

namespace LifeChart;

public partial class App : MauiApplication
{
    public AppSettings Settings { get; }

    public App(ISettingsService settingsService, IServiceProvider services)
    {
        InitializeComponent();
        Settings = settingsService.Load();

        // Reihenfolge: Onboarding → Lock → Shell
        if (Settings.EveningReminderTime is null)
        {
            var onboardingPage = services.GetRequiredService<OnboardingPage>();
            MainPage = new NavigationPage(onboardingPage);
        }
        else if (Settings.BiometricsEnabled)
        {
            MainPage = services.GetRequiredService<LockPage>();
        }
        else
        {
            MainPage = services.GetRequiredService<AppShell>();
        }
    }
}
