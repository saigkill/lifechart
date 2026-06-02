namespace LifeChart.Application.Settings;

public enum CloudProvider { None, GoogleDrive, Nextcloud, LocalExport }
public enum InputMode { Quick, Full, AlwaysAsk }
public enum AppLanguage { System, German, English }

public class AppSettings
{
    // Backup
    public CloudProvider BackupProvider { get; set; } = CloudProvider.None;
    public string? NextcloudUrl { get; set; }
    public string? NextcloudUsername { get; set; }
    public string? NextcloudPassword { get; set; }
    public string? GoogleDriveAccessToken { get; set; }
    public string? GoogleDriveRefreshToken { get; set; }
    public bool AutoBackupEnabled { get; set; } = true;
    public int BackupWarningThresholdDays { get; set; } = 7;
    public DateTime? LastBackupAt { get; set; }

    // Benachrichtigungen
    public bool EveningReminderEnabled { get; set; } = true;
    public TimeOnly? EveningReminderTime { get; set; } = null;
    public bool CrisisHintEnabled { get; set; } = true;

    // UX
    public InputMode InputMode { get; set; } = InputMode.Quick;
    public bool ColorBlindModeEnabled { get; set; } = true;

    // App
    public bool BiometricsEnabled { get; set; } = false;
    public AppLanguage Language { get; set; } = AppLanguage.System;
}
