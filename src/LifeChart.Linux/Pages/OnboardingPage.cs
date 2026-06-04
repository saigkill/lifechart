using System.Diagnostics;
using LifeChart.Application.Interfaces;
using LifeChart.Application.Localization;
using LifeChart.Application.Settings;
using LifeChart.Application.UseCases.Medications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace LifeChart.Linux.Pages;

public class OnboardingPage : ContentPage
{
    private readonly Microsoft.Maui.Controls.Switch _reminderSwitch = new() { IsToggled = true };
    private readonly TimePicker _reminderTimePicker = new()
    {
        Time = new TimeSpan(20, 0, 0)
    };
    private readonly HorizontalStackLayout _timeRow = new() { Spacing = 8 };

    public OnboardingPage()
    {
        Title = L.Onboarding_Title;

        _reminderSwitch.Toggled += (_, e) => _timeRow.IsVisible = e.Value;

        _timeRow.Children.Add(new Label { Text = L.Common_Time, VerticalOptions = LayoutOptions.Center });
        _timeRow.Children.Add(_reminderTimePicker);

        var completeButton = new Button
        {
            Text = L.Onboarding_Start,
            BackgroundColor = Color.FromArgb("#0072B2"),
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 16, 0, 0),
        };
        completeButton.Clicked += OnCompleteClicked;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(24),
                Spacing = 20,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label
                    {
                        Text = L.Onboarding_Welcome,
                        FontSize = 24,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center,
                    },
                    new Label
                    {
                        Text = L.Onboarding_Subtitle,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Colors.Gray,
                    },

                    new BoxView { HeightRequest = 1, Color = Colors.LightGray, Margin = new Thickness(0, 8) },

                    BuildDisclaimerBox(),

                    new BoxView { HeightRequest = 1, Color = Colors.LightGray, Margin = new Thickness(0, 8) },

                    new Label
                    {
                        Text = L.Onboarding_ReminderQuestion,
                        FontAttributes = FontAttributes.Bold,
                    },
                    new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children =
                        {
                            _reminderSwitch,
                            new Label { Text = L.Settings_EveningReminderEnable, VerticalOptions = LayoutOptions.Center },
                        }
                    },
                    _timeRow,

                    new Label
                    {
                        Text = L.Onboarding_ReminderNote,
                        FontSize = 11,
                        TextColor = Colors.Gray,
                        FontAttributes = FontAttributes.Italic,
                    },

                    completeButton,
                }
            }
        };
    }

    private async void OnCompleteClicked(object? sender, EventArgs e)
    {
        var settingsService = IPlatformApplication.Current!.Services
            .GetRequiredService<ISettingsService>();

        var settings = settingsService.Load();
        settings.EveningReminderEnabled = _reminderSwitch.IsToggled;

        var t = _reminderTimePicker.Time ?? new TimeSpan(20, 0, 0);
        // Immer eine Zeit setzen (Fallback 20:00) damit der null-Check beim nächsten
        // App-Start nicht erneut Onboarding auslöst.
        settings.EveningReminderTime = new TimeOnly(t.Hours, t.Minutes);

        await settingsService.SaveAsync(settings);

        // Evening-Reminder-Timer einrichten (keine Medikamente beim ersten Start)
        var alarmService = IPlatformApplication.Current!.Services
            .GetRequiredService<IAlarmService>();
        var emptyMeds = await IPlatformApplication.Current.Services
            .GetRequiredService<GetActiveMedicationsUseCase>()
            .ExecuteAsync();
        await alarmService.ScheduleAsync(emptyMeds);

        // GTK4 kann window.Page nicht korrekt neu messen und OpenWindow
        // schlägt auf manchen Systemen wegen GL-Kontext fehl.
        // Zuverlässigste Lösung: Prozess via /proc/self/cmdline neu starten.
        RestartProcess();
    }

    private static void RestartProcess()
    {
        try
        {
            // /proc/self/cmdline enthält die exakte Startkommandozeile (null-separiert)
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

    private static View BuildDisclaimerBox() =>
        new Border
        {
            Padding = new Thickness(16),
            BackgroundColor = Color.FromArgb("#FFF8E1"),
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    new Label { Text = L.Onboarding_DisclaimerTitle, FontAttributes = FontAttributes.Bold },
                    new Label
                    {
                        Text = L.Onboarding_Disclaimer,
                        FontSize = 13,
                    },
                    new Label
                    {
                        Text = L.Onboarding_CrisisHotline,
                        FontSize = 13,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#0072B2"),
                    },
                }
            }
        };
}
