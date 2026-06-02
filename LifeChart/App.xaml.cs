using LifeChart.Application.Settings;
using LifeChart.Pages;

namespace LifeChart;

public partial class App : Application
{
    public AppSettings Settings { get; }

    public App(ISettingsService settingsService, OnboardingPage onboardingPage, AppShell shell)
    {
        InitializeComponent();
        Settings = settingsService.Load();

        // Onboarding wenn Abend-Erinnerung noch nicht konfiguriert
        MainPage = Settings.EveningReminderTime is null
            ? new NavigationPage(onboardingPage)
            : shell;
    }
}
