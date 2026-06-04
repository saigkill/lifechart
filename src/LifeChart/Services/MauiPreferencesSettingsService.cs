using LifeChart.Application.Settings;

namespace LifeChart.Services;

public class MauiPreferencesSettingsService : ISettingsService
{
    private const string KeyBackupProvider = "backup_provider";
    private const string KeyNextcloudUrl = "nextcloud_url";
    private const string KeyNextcloudUsername = "nextcloud_username";
    private const string KeyNextcloudPassword = "nextcloud_password";
    private const string KeyGoogleAccessToken = "google_access_token";
    private const string KeyGoogleRefreshToken = "google_refresh_token";
    private const string KeyAutoBackupEnabled = "auto_backup_enabled";
    private const string KeyBackupWarningDays = "backup_warning_days";
    private const string KeyLastBackupAt = "last_backup_at";
    private const string KeyEveningReminderEnabled = "evening_reminder_enabled";
    private const string KeyEveningReminderTime = "evening_reminder_time";
    private const string KeyCrisisHintEnabled = "crisis_hint_enabled";
    private const string KeyInputMode = "input_mode";
    private const string KeyColorBlindMode = "color_blind_mode";
    private const string KeyBiometricsEnabled = "biometrics_enabled";
    private const string KeyLanguage = "language";

    public AppSettings Load()
    {
        var reminderTimeStr = Preferences.Get(KeyEveningReminderTime, string.Empty);
        var lastBackupStr = Preferences.Get(KeyLastBackupAt, string.Empty);

        return new AppSettings
        {
            BackupProvider = Enum.TryParse<CloudProvider>(
                Preferences.Get(KeyBackupProvider, nameof(CloudProvider.None)),
                out var provider) ? provider : CloudProvider.None,

            NextcloudUrl = Preferences.Get(KeyNextcloudUrl, null as string),
            NextcloudUsername = Preferences.Get(KeyNextcloudUsername, null as string),
            NextcloudPassword = Preferences.Get(KeyNextcloudPassword, null as string),
            GoogleDriveAccessToken = Preferences.Get(KeyGoogleAccessToken, null as string),
            GoogleDriveRefreshToken = Preferences.Get(KeyGoogleRefreshToken, null as string),

            AutoBackupEnabled = Preferences.Get(KeyAutoBackupEnabled, true),
            BackupWarningThresholdDays = Preferences.Get(KeyBackupWarningDays, 7),

            LastBackupAt = string.IsNullOrEmpty(lastBackupStr)
                ? null
                : DateTime.TryParse(lastBackupStr, out var dt) ? dt : null,

            EveningReminderEnabled = Preferences.Get(KeyEveningReminderEnabled, true),

            EveningReminderTime = string.IsNullOrEmpty(reminderTimeStr)
                ? null
                : TimeOnly.TryParse(reminderTimeStr, out var t) ? t : null,

            CrisisHintEnabled = Preferences.Get(KeyCrisisHintEnabled, true),

            InputMode = Enum.TryParse<InputMode>(
                Preferences.Get(KeyInputMode, nameof(InputMode.Quick)),
                out var inputMode) ? inputMode : InputMode.Quick,

            ColorBlindModeEnabled = Preferences.Get(KeyColorBlindMode, true),
            BiometricsEnabled = Preferences.Get(KeyBiometricsEnabled, false),

            Language = Enum.TryParse<AppLanguage>(
                Preferences.Get(KeyLanguage, nameof(AppLanguage.System)),
                out var lang) ? lang : AppLanguage.System
        };
    }

    public Task SaveAsync(AppSettings settings)
    {
        Preferences.Set(KeyBackupProvider, settings.BackupProvider.ToString());
        Preferences.Set(KeyNextcloudUrl, settings.NextcloudUrl ?? string.Empty);
        Preferences.Set(KeyNextcloudUsername, settings.NextcloudUsername ?? string.Empty);
        Preferences.Set(KeyNextcloudPassword, settings.NextcloudPassword ?? string.Empty);
        Preferences.Set(KeyGoogleAccessToken, settings.GoogleDriveAccessToken ?? string.Empty);
        Preferences.Set(KeyGoogleRefreshToken, settings.GoogleDriveRefreshToken ?? string.Empty);
        Preferences.Set(KeyAutoBackupEnabled, settings.AutoBackupEnabled);
        Preferences.Set(KeyBackupWarningDays, settings.BackupWarningThresholdDays);
        Preferences.Set(KeyLastBackupAt,
            settings.LastBackupAt?.ToString("O") ?? string.Empty);
        Preferences.Set(KeyEveningReminderEnabled, settings.EveningReminderEnabled);
        Preferences.Set(KeyEveningReminderTime,
            settings.EveningReminderTime?.ToString("HH:mm") ?? string.Empty);
        Preferences.Set(KeyCrisisHintEnabled, settings.CrisisHintEnabled);
        Preferences.Set(KeyInputMode, settings.InputMode.ToString());
        Preferences.Set(KeyColorBlindMode, settings.ColorBlindModeEnabled);
        Preferences.Set(KeyBiometricsEnabled, settings.BiometricsEnabled);
        Preferences.Set(KeyLanguage, settings.Language.ToString());

        return Task.CompletedTask;
    }
}
