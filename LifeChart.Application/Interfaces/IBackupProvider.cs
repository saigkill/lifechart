namespace LifeChart.Application.Interfaces;

public record BackupInfo(string FileName, DateTime CreatedAt, long SizeBytes);

public interface IBackupProvider
{
    Task BackupAsync(string dbPath);
    Task<IEnumerable<BackupInfo>> ListBackupsAsync();
    Task RestoreAsync(BackupInfo backup, string targetPath);
}
