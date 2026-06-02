using LifeChart.Application.Interfaces;
using LifeChart.Application.Settings;

namespace LifeChart.Application.UseCases.Backup;

public class CreateBackupUseCase
{
    private readonly IBackupProvider _backupProvider;
    private readonly ISettingsService _settings;
    private readonly string _dbPath;

    public CreateBackupUseCase(
        IBackupProvider backupProvider,
        ISettingsService settings,
        string dbPath)
    {
        _backupProvider = backupProvider;
        _settings = settings;
        _dbPath = dbPath;
    }

    public async Task ExecuteAsync()
    {
        var settings = _settings.Load();
        if (!settings.AutoBackupEnabled) return;

        if (settings.LastBackupAt.HasValue &&
            DateTime.UtcNow - settings.LastBackupAt.Value < TimeSpan.FromHours(24))
            return;

        await _backupProvider.BackupAsync(_dbPath);
        settings.LastBackupAt = DateTime.UtcNow;
        await _settings.SaveAsync(settings);
    }
}
