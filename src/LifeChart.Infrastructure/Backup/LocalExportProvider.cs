using LifeChart.Application.Interfaces;

namespace LifeChart.Infrastructure.Backup;

public class LocalExportProvider : IBackupProvider
{
    private readonly string _backupDir;

    public LocalExportProvider()
    {
        _backupDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "LifeChart", "Backups");

        Directory.CreateDirectory(_backupDir);
    }

    public async Task BackupAsync(string dbPath)
    {
        var fileName = $"lifechart_backup_{DateTime.Today:yyyy-MM-dd}.sqlite";
        var destPath = Path.Combine(_backupDir, fileName);
        await CopyFileAsync(dbPath, destPath);
    }

    public Task<IEnumerable<BackupInfo>> ListBackupsAsync()
    {
        if (!Directory.Exists(_backupDir))
            return Task.FromResult(Enumerable.Empty<BackupInfo>());

        var files = Directory
            .GetFiles(_backupDir, "lifechart_backup_*.sqlite")
            .Select(path =>
            {
                var info = new FileInfo(path);
                return new BackupInfo(info.Name, info.LastWriteTimeUtc, info.Length);
            })
            .OrderByDescending(b => b.CreatedAt);

        return Task.FromResult<IEnumerable<BackupInfo>>(files);
    }

    public async Task RestoreAsync(BackupInfo backup, string targetPath)
    {
        var sourcePath = Path.Combine(_backupDir, backup.FileName);
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException(
                $"Backup-Datei nicht gefunden: {backup.FileName}");

        await CopyFileAsync(sourcePath, targetPath);
    }

    public string BackupDirectory => _backupDir;

    private static async Task CopyFileAsync(string source, string destination)
    {
        await using var src = File.OpenRead(source);
        await using var dst = File.Create(destination);
        await src.CopyToAsync(dst);
    }
}
