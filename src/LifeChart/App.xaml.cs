using LifeChart.Application.Settings;
using LifeChart.Pages;
using MauiApplication = Microsoft.Maui.Controls.Application;

namespace LifeChart;

public partial class App : MauiApplication
{
    public AppSettings Settings { get; }

    public App(
        ISettingsService settingsService,
        OnboardingPage onboardingPage,
        LockPage lockPage,
        AppShell shell)
    {
        InitializeComponent();
        Settings = settingsService.Load();

        // Reihenfolge: Onboarding → Lock → Shell
        if (Settings.EveningReminderTime is null)
        {
            MainPage = new NavigationPage(onboardingPage);
        }
        else if (Settings.BiometricsEnabled)
        {
            MainPage = lockPage;
        }
        else
        {
            MainPage = shell;
        }
    }
}
