using LifeChart.Application.Interfaces;

namespace LifeChart.Infrastructure.Backup;

#if GOOGLE_SERVICES
public class GoogleDriveBackupProvider : IBackupProvider
{
    public Task BackupAsync(string dbPath) => throw new NotImplementedException();
    public Task<IEnumerable<BackupInfo>> ListBackupsAsync() => throw new NotImplementedException();
    public Task RestoreAsync(BackupInfo backup, string targetPath) => throw new NotImplementedException();
}
#endif
