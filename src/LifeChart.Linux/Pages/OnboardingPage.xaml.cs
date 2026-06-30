using System.Diagnostics;
using LifeChart.Application.Interfaces;
using LifeChart.Application.Localization;
using LifeChart.Application.Settings;
using LifeChart.Application.UseCases.Medications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace LifeChart.Linux.Pages;

public partial class OnboardingPage : ContentPage
{
    public OnboardingPage()
    {
        InitializeComponent();
        Title = L.Onboarding_Title;
        WelcomeLabel.Text = L.Onboarding_Welcome;
        SubtitleLabel.Text = L.Onboarding_Subtitle;
        DisclaimerTitleLabel.Text = L.Onboarding_DisclaimerTitle;
        DisclaimerLabel.Text = L.Onboarding_Disclaimer;
        CrisisHotlineLabel.Text = L.Onboarding_CrisisHotline;
        ReminderQuestionLabel.Text = L.Onboarding_ReminderQuestion;
        ReminderEnableLabel.Text = L.Settings_EveningReminderEnable;
        TimeLabel.Text = L.Common_Time;
        ReminderNoteLabel.Text = L.Onboarding_ReminderNote;
        CompleteButton.Text = L.Onboarding_Start;
    }

    private void OnReminderSwitchToggled(object? sender, ToggledEventArgs e)
        => TimeRow.IsVisible = e.Value;

    private async void OnCompleteClicked(object? sender, EventArgs e)
    {
        var settingsService = IPlatformApplication.Current!.Services
            .GetRequiredService<ISettingsService>();

        var settings = settingsService.Load();
        settings.EveningReminderEnabled = ReminderSwitch.IsToggled;

        var t = ReminderTimePicker.Time ?? new TimeSpan(20, 0, 0);
        settings.EveningReminderTime = new TimeOnly(t.Hours, t.Minutes);

        await settingsService.SaveAsync(settings);

        var alarmService = IPlatformApplication.Current!.Services
            .GetRequiredService<IAlarmService>();
        var emptyMeds = await IPlatformApplication.Current.Services
            .GetRequiredService<GetActiveMedicationsUseCase>()
            .ExecuteAsync();
        await alarmService.ScheduleAsync(emptyMeds);

        RestartProcess();
    }

    private static void RestartProcess()
    {
        try
        {
            var parts = File.ReadAllText("/proc/self/cmdline")
                .Split('\0', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                var psi = new ProcessStartInfo(parts[0]) { UseShellExecute = false };
                foreach (var arg in parts.Skip(1))
                    psi.ArgumentList.Add(arg);
                Process.Start(psi);
            }
        }
        catch
        {
            // Fallback: nur beenden, Nutzer startet manuell neu
        }
        finally
        {
            Environment.Exit(0);
        }
    }
}
