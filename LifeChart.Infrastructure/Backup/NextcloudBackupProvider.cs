using LifeChart.Application.Interfaces;

namespace LifeChart.Infrastructure.Backup;

public class NextcloudBackupProvider : IBackupProvider
{
    private readonly string _serverUrl;

    public NextcloudBackupProvider(string serverUrl)
        => _serverUrl = serverUrl;

    public Task BackupAsync(string dbPath) => throw new NotImplementedException();
    public Task<IEnumerable<BackupInfo>> ListBackupsAsync() => throw new NotImplementedException();
    public Task RestoreAsync(BackupInfo backup, string targetPath) => throw new NotImplementedException();
}
