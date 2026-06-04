using LifeChart.Application.Interfaces;
using LifeChart.Application.Localization;
using LifeChart.Application.Settings;
using LifeChart.Application.UseCases.Medications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LifeChart.Linux.Pages;

public class SettingsPage : ContentPage
{
    // Backup
    private readonly Picker _backupProviderPicker = new();
    private readonly VerticalStackLayout _nextcloudSection = new() { Spacing = 8, IsVisible = false };
    private readonly Entry _nextcloudUrlEntry = new() { Placeholder = "https://cloud.example.com", Keyboard = Keyboard.Url };
    private readonly Entry _nextcloudUserEntry = new();
    private readonly Entry _nextcloudPassEntry = new() { IsPassword = true };
    private readonly Switch _autoBackupSwitch = new();
    private readonly Entry _backupWarningDaysEntry = new() { Keyboard = Keyboard.Numeric, WidthRequest = 60 };

    // Erinnerungen
    private readonly Switch _eveningReminderSwitch = new();
    private readonly HorizontalStackLayout _reminderTimeRow = new() { Spacing = 8, IsVisible = false };
    private readonly TimePicker _reminderTimePicker = new();
    private readonly Switch _crisisHintSwitch = new();

    // Darstellung
    private readonly Picker _inputModePicker = new();
    private readonly Switch _colorBlindSwitch = new();

    // App
    private readonly Picker _languagePicker = new();

    private readonly Label _statusLabel = new() { HorizontalOptions = LayoutOptions.Center };

    private static readonly CloudProvider[] BackupProviderValues = [CloudProvider.None, CloudProvider.Nextcloud, CloudProvider.LocalExport];
    private static readonly InputMode[] InputModeValues = [InputMode.Quick, InputMode.Full, InputMode.AlwaysAsk];
    private static readonly AppLanguage[] LanguageValues = [AppLanguage.System, AppLanguage.German, AppLanguage.English];

    public SettingsPage()
    {
        Title = L.Settings_Title;

        _nextcloudUserEntry.Placeholder = L.Common_Username;
        _nextcloudPassEntry.Placeholder = L.Common_Password;

        var backupProviderLabels = new[] { L.Settings_NoBackup, "Nextcloud", L.Settings_LocalExport };
        var inputModeLabels = new[] { L.Settings_InputQuick, L.Settings_InputFull, L.Settings_InputAlwaysAsk };
        var languageLabels = new[] { L.Settings_LangSystem, L.Settings_LangGerman, L.Settings_LangEnglish };

        foreach (var l in backupProviderLabels) _backupProviderPicker.Items.Add(l);
        foreach (var l in inputModeLabels) _inputModePicker.Items.Add(l);
        foreach (var l in languageLabels) _languagePicker.Items.Add(l);

        _backupProviderPicker.SelectedIndexChanged += (_, _) =>
        {
            var isNextcloud = SelectedBackupProvider() == CloudProvider.Nextcloud;
            _nextcloudSection.IsVisible = isNextcloud;
        };

        _eveningReminderSwitch.Toggled += (_, e) =>
            _reminderTimeRow.IsVisible = e.Value;

        _reminderTimeRow.Children.Add(new Label { Text = L.Common_Time, VerticalOptions = LayoutOptions.Center });
        _reminderTimeRow.Children.Add(_reminderTimePicker);

        _nextcloudSection.Children.Add(MakeField(L.Settings_NextcloudUrl, _nextcloudUrlEntry));
        _nextcloudSection.Children.Add(MakeField(L.Common_Username, _nextcloudUserEntry));
        _nextcloudSection.Children.Add(MakeField(L.Common_Password, _nextcloudPassEntry));

        var saveButton = new Button
        {
            Text = L.Settings_SaveButton,
            HorizontalOptions = LayoutOptions.Center,
            BackgroundColor = Color.FromArgb("#0072B2"),
            TextColor = Colors.White,
        };
        saveButton.Clicked += OnSaveClicked;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Padding = new Thickness(24, 16),
                Children =
                {
                    // Backup
                    MakeSectionHeader(L.Settings_Backup),
                    MakeField(L.Settings_CloudProvider, _backupProviderPicker),
                    _nextcloudSection,
                    MakeSwitchRow(L.Settings_AutoBackup, _autoBackupSwitch),
                    new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children =
                        {
                            new Label { Text = L.Settings_BackupWarningPrefix, VerticalOptions = LayoutOptions.Center },
                            _backupWarningDaysEntry,
                            new Label { Text = L.Settings_BackupWarningSuffix, VerticalOptions = LayoutOptions.Center },
                        }
                    },

                    MakeDivider(),

                    // Erinnerungen
                    MakeSectionHeader(L.Settings_Reminders),
                    MakeSwitchRow(L.Settings_EveningReminder, _eveningReminderSwitch),
                    _reminderTimeRow,
                    MakeSwitchRow(L.Settings_CrisisHint, _crisisHintSwitch),

                    MakeDivider(),

                    // Darstellung
                    MakeSectionHeader(L.Settings_Display),
                    MakeField(L.Settings_InputMode, _inputModePicker),
                    MakeSwitchRow(L.Settings_ColorBlind, _colorBlindSwitch),

                    MakeDivider(),

                    // App
                    MakeSectionHeader(L.Settings_App),
                    MakeField(L.Settings_Language, _languagePicker),

                    new BoxView { HeightRequest = 8 },
                    saveButton,
                    _statusLabel,
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadSettings();
    }

    private void LoadSettings()
    {
        var s = GetService<ISettingsService>().Load();

        _backupProviderPicker.SelectedIndex = Array.IndexOf(BackupProviderValues, s.BackupProvider);
        _nextcloudUrlEntry.Text = s.NextcloudUrl ?? string.Empty;
        _nextcloudUserEntry.Text = s.NextcloudUsername ?? string.Empty;
        _nextcloudPassEntry.Text = s.NextcloudPassword ?? string.Empty;
        _autoBackupSwitch.IsToggled = s.AutoBackupEnabled;
        _backupWarningDaysEntry.Text = s.BackupWarningThresholdDays.ToString();
        _nextcloudSection.IsVisible = s.BackupProvider == CloudProvider.Nextcloud;

        _eveningReminderSwitch.IsToggled = s.EveningReminderEnabled;
        _reminderTimePicker.Time = s.EveningReminderTime.HasValue
            ? new TimeSpan(s.EveningReminderTime.Value.Hour, s.EveningReminderTime.Value.Minute, 0)
            : new TimeSpan(20, 0, 0);
        _reminderTimeRow.IsVisible = s.EveningReminderEnabled;
        _crisisHintSwitch.IsToggled = s.CrisisHintEnabled;

        _inputModePicker.SelectedIndex = Array.IndexOf(InputModeValues, s.InputMode);
        _colorBlindSwitch.IsToggled = s.ColorBlindModeEnabled;

        _languagePicker.SelectedIndex = Array.IndexOf(LanguageValues, s.Language);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var t = _reminderTimePicker.Time ?? TimeSpan.Zero;

        var settings = new AppSettings
        {
            BackupProvider = SelectedBackupProvider(),
            NextcloudUrl = NullIfEmpty(_nextcloudUrlEntry.Text),
            NextcloudUsername = NullIfEmpty(_nextcloudUserEntry.Text),
            NextcloudPassword = NullIfEmpty(_nextcloudPassEntry.Text),
            AutoBackupEnabled = _autoBackupSwitch.IsToggled,
            BackupWarningThresholdDays = int.TryParse(_backupWarningDaysEntry.Text, out var d) ? d : 7,

            EveningReminderEnabled = _eveningReminderSwitch.IsToggled,
            EveningReminderTime = _eveningReminderSwitch.IsToggled
                ? new TimeOnly(t.Hours, t.Minutes)
                : null,
            CrisisHintEnabled = _crisisHintSwitch.IsToggled,

            InputMode = _inputModePicker.SelectedIndex >= 0
                ? InputModeValues[_inputModePicker.SelectedIndex]
                : InputMode.Quick,
            ColorBlindModeEnabled = _colorBlindSwitch.IsToggled,

            BiometricsEnabled = false,

            Language = _languagePicker.SelectedIndex >= 0
                ? LanguageValues[_languagePicker.SelectedIndex]
                : AppLanguage.System,
        };

        await GetService<ISettingsService>().SaveAsync(settings);

        // Abend-Erinnerung und Medikamenten-Alarme neu planen
        var allMeds = await GetService<GetActiveMedicationsUseCase>().ExecuteAsync();
        await GetService<IAlarmService>().ScheduleAsync(allMeds);

        _statusLabel.TextColor = Colors.Green;
        _statusLabel.Text = L.Settings_Saved;
    }

    private CloudProvider SelectedBackupProvider() =>
        _backupProviderPicker.SelectedIndex >= 0
            ? BackupProviderValues[_backupProviderPicker.SelectedIndex]
            : CloudProvider.None;

    private static string? NullIfEmpty(string? s) =>
        string.IsNullOrWhiteSpace(s) ? null : s;

    private static T GetService<T>() where T : notnull =>
        IPlatformApplication.Current!.Services.GetRequiredService<T>();

    private static Label MakeSectionHeader(string text) => new()
    {
        Text = text,
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
    };

    private static BoxView MakeDivider() => new()
    {
        HeightRequest = 1,
        Color = Colors.LightGray,
        Margin = new Thickness(0, 4),
    };

    private static View MakeField(string label, View control) =>
        new VerticalStackLayout
        {
            Spacing = 4,
            Children = { new Label { Text = label, FontSize = 13 }, control }
        };

    private static View MakeSwitchRow(string label, Switch sw) =>
        new HorizontalStackLayout
        {
            Spacing = 12,
            Children =
            {
                sw,
                new Label { Text = label, FontSize = 14, VerticalOptions = LayoutOptions.Center },
            }
        };
}
