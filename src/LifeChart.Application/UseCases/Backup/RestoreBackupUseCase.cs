using LifeChart.Application.Interfaces;

namespace LifeChart.Application.UseCases.Backup;

public class RestoreBackupUseCase
{
    private readonly IBackupProvider _backupProvider;
    private readonly string _dbPath;

    public RestoreBackupUseCase(IBackupProvider backupProvider, string dbPath)
    {
        _backupProvider = backupProvider;
        _dbPath = dbPath;
    }

    public async Task<IEnumerable<BackupInfo>> ListAvailableAsync()
        => await _backupProvider.ListBackupsAsync();

    public async Task ExecuteAsync(BackupInfo backup)
        => await _backupProvider.RestoreAsync(backup, _dbPath);
}
