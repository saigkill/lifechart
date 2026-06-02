using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeChart.Application.Settings;

namespace LifeChart.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty] private CloudProvider _backupProvider;
    [ObservableProperty] private string? _nextcloudUrl;
    [ObservableProperty] private bool _autoBackupEnabled;
    [ObservableProperty] private int _backupWarningThresholdDays;
    [ObservableProperty] private bool _eveningReminderEnabled;
    [ObservableProperty] private TimeOnly? _eveningReminderTime;
    [ObservableProperty] private bool _crisisHintEnabled;
    [ObservableProperty] private InputMode _inputMode;
    [ObservableProperty] private bool _colorBlindModeEnabled;
    [ObservableProperty] private bool _biometricsEnabled;
    [ObservableProperty] private AppLanguage _language;

    public CloudProvider[] CloudProviders { get; } = Enum.GetValues<CloudProvider>();
    public InputMode[] InputModes { get; } = Enum.GetValues<InputMode>();
    public AppLanguage[] Languages { get; } = Enum.GetValues<AppLanguage>();

    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        Load();
    }

    private void Load()
    {
        var s = _settingsService.Load();
        BackupProvider = s.BackupProvider;
        NextcloudUrl = s.NextcloudUrl;
        AutoBackupEnabled = s.AutoBackupEnabled;
        BackupWarningThresholdDays = s.BackupWarningThresholdDays;
        EveningReminderEnabled = s.EveningReminderEnabled;
        EveningReminderTime = s.EveningReminderTime;
        CrisisHintEnabled = s.CrisisHintEnabled;
        InputMode = s.InputMode;
        ColorBlindModeEnabled = s.ColorBlindModeEnabled;
        BiometricsEnabled = s.BiometricsEnabled;
        Language = s.Language;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var settings = new AppSettings
        {
            BackupProvider = BackupProvider,
            NextcloudUrl = NextcloudUrl,
            AutoBackupEnabled = AutoBackupEnabled,
            BackupWarningThresholdDays = BackupWarningThresholdDays,
            EveningReminderEnabled = EveningReminderEnabled,
            EveningReminderTime = EveningReminderTime,
            CrisisHintEnabled = CrisisHintEnabled,
            InputMode = InputMode,
            ColorBlindModeEnabled = ColorBlindModeEnabled,
            BiometricsEnabled = BiometricsEnabled,
            Language = Language
        };
        await _settingsService.SaveAsync(settings);
    }
}
