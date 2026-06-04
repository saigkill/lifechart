using LifeChart.Application.Settings;
using LifeChart.Linux.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace LifeChart;

public class LinuxApp : Microsoft.Maui.Controls.Application
{
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var settings = IPlatformApplication.Current!.Services
            .GetRequiredService<ISettingsService>()
            .Load();

        var startPage = settings.EveningReminderTime is null
            ? (Page)new OnboardingPage()
            : BuildMainTabs();

        return new Window(startPage)
        {
            Title = "LifeChart [Experimentell/Linux]"
        };
    }

    public static TabbedPage BuildMainTabs() => new()
    {
        Children =
        {
            new TodayPage(),
            new ChartPage(),
            new NavigationPage(new MedicationsPage()) { Title = "Medikamente" },
            new CrisisPage(),
            new SettingsPage(),
            new AboutPage(),
        }
    };
}
