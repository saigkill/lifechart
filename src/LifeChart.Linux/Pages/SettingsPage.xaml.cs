using LifeChart.Application.Interfaces;
using LifeChart.Application.Localization;
using LifeChart.Application.Settings;
using LifeChart.Application.UseCases.Medications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LifeChart.Linux.Pages;

public partial class SettingsPage : ContentPage
{
    private static readonly CloudProvider[] BackupProviderValues = [CloudProvider.None, CloudProvider.Nextcloud, CloudProvider.LocalExport];
    private static readonly InputMode[] InputModeValues = [InputMode.Quick, InputMode.Full, InputMode.AlwaysAsk];
    private static readonly AppLanguage[] LanguageValues = [AppLanguage.System, AppLanguage.German, AppLanguage.English];

    public SettingsPage()
    {
        InitializeComponent();
        Title = L.Settings_Title;

        // Section headers
        BackupSectionLabel.Text = L.Settings_Backup;
        RemindersSectionLabel.Text = L.Settings_Reminders;
        DisplaySectionLabel.Text = L.Settings_Display;
        AppSectionLabel.Text = L.Settings_App;

        // Field labels
        CloudProviderLabel.Text = L.Settings_CloudProvider;
        NextcloudUrlLabel.Text = L.Settings_NextcloudUrl;
        NextcloudUrlEntry.Placeholder = "https://cloud.example.com";
        NextcloudUserLabel.Text = L.Common_Username;
        NextcloudUserEntry.Placeholder = L.Common_Username;
        NextcloudPassLabel.Text = L.Common_Password;
        NextcloudPassEntry.Placeholder = L.Common_Password;
        AutoBackupLabel.Text = L.Settings_AutoBackup;
        BackupWarningPrefixLabel.Text = L.Settings_BackupWarningPrefix;
        BackupWarningSuffixLabel.Text = L.Settings_BackupWarningSuffix;
        EveningReminderLabel.Text = L.Settings_EveningReminder;
        ReminderTimeLabel.Text = L.Common_Time;
        CrisisHintLabel.Text = L.Settings_CrisisHint;
        InputModeLabel.Text = L.Settings_InputMode;
        ColorBlindLabel.Text = L.Settings_ColorBlind;
        LanguageLabel.Text = L.Settings_Language;
        SaveButton.Text = L.Settings_SaveButton;

        // Populate pickers
        foreach (var l in new[] { L.Settings_NoBackup, "Nextcloud", L.Settings_LocalExport })
            BackupProviderPicker.Items.Add(l);
        foreach (var l in new[] { L.Settings_InputQuick, L.Settings_InputFull, L.Settings_InputAlwaysAsk })
            InputModePicker.Items.Add(l);
        foreach (var l in new[] { L.Settings_LangSystem, L.Settings_LangGerman, L.Settings_LangEnglish })
            LanguagePicker.Items.Add(l);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadSettings();
    }

    private void OnBackupProviderChanged(object? sender, EventArgs e)
    {
        var isNextcloud = BackupProviderPicker.SelectedIndex >= 0
            && BackupProviderValues[BackupProviderPicker.SelectedIndex] == CloudProvider.Nextcloud;
        NextcloudSection.IsVisible = isNextcloud;
    }

    private void OnEveningReminderToggled(object? sender, ToggledEventArgs e)
        => ReminderTimeRow.IsVisible = e.Value;

    private void LoadSettings()
    {
        var s = GetService<ISettingsService>().Load();

        BackupProviderPicker.SelectedIndex = Array.IndexOf(BackupProviderValues, s.BackupProvider);
        NextcloudUrlEntry.Text = s.NextcloudUrl ?? string.Empty;
        NextcloudUserEntry.Text = s.NextcloudUsername ?? string.Empty;
        NextcloudPassEntry.Text = s.NextcloudPassword ?? string.Empty;
        AutoBackupSwitch.IsToggled = s.AutoBackupEnabled;
        BackupWarningDaysEntry.Text = s.BackupWarningThresholdDays.ToString();
        NextcloudSection.IsVisible = s.BackupProvider == CloudProvider.Nextcloud;

        EveningReminderSwitch.IsToggled = s.EveningReminderEnabled;
        ReminderTimePicker.Time = s.EveningReminderTime.HasValue
            ? new TimeSpan(s.EveningReminderTime.Value.Hour, s.EveningReminderTime.Value.Minute, 0)
            : new TimeSpan(20, 0, 0);
        ReminderTimeRow.IsVisible = s.EveningReminderEnabled;
        CrisisHintSwitch.IsToggled = s.CrisisHintEnabled;

        InputModePicker.SelectedIndex = Array.IndexOf(InputModeValues, s.InputMode);
        ColorBlindSwitch.IsToggled = s.ColorBlindModeEnabled;

        LanguagePicker.SelectedIndex = Array.IndexOf(LanguageValues, s.Language);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var t = ReminderTimePicker.Time ?? TimeSpan.Zero;

        var settings = new AppSettings
        {
            BackupProvider = BackupProviderPicker.SelectedIndex >= 0
                ? BackupProviderValues[BackupProviderPicker.SelectedIndex]
                : CloudProvider.None,
            NextcloudUrl = NullIfEmpty(NextcloudUrlEntry.Text),
            NextcloudUsername = NullIfEmpty(NextcloudUserEntry.Text),
            NextcloudPassword = NullIfEmpty(NextcloudPassEntry.Text),
            AutoBackupEnabled = AutoBackupSwitch.IsToggled,
            BackupWarningThresholdDays = int.TryParse(BackupWarningDaysEntry.Text, out var d) ? d : 7,

            EveningReminderEnabled = EveningReminderSwitch.IsToggled,
            EveningReminderTime = EveningReminderSwitch.IsToggled
                ? new TimeOnly(t.Hours, t.Minutes)
                : null,
            CrisisHintEnabled = CrisisHintSwitch.IsToggled,

            InputMode = InputModePicker.SelectedIndex >= 0
                ? InputModeValues[InputModePicker.SelectedIndex]
                : InputMode.Quick,
            ColorBlindModeEnabled = ColorBlindSwitch.IsToggled,

            BiometricsEnabled = false,

            Language = LanguagePicker.SelectedIndex >= 0
                ? LanguageValues[LanguagePicker.SelectedIndex]
                : AppLanguage.System,
        };

        await GetService<ISettingsService>().SaveAsync(settings);

        var allMeds = await GetService<GetActiveMedicationsUseCase>().ExecuteAsync();
        await GetService<IAlarmService>().ScheduleAsync(allMeds);

        StatusLabel.TextColor = Colors.Green;
        StatusLabel.Text = L.Settings_Saved;
    }

    private static string? NullIfEmpty(string? s) =>
        string.IsNullOrWhiteSpace(s) ? null : s;

    private static T GetService<T>() where T : notnull =>
        IPlatformApplication.Current!.Services.GetRequiredService<T>();
}
