using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using LifeChart.Application.Interfaces;
using LifeChart.Application.Settings;

namespace LifeChart.Infrastructure.Backup;

// Google Drive REST API v3
// Dokumentation: https://developers.google.com/drive/api/v3/reference
#if GOOGLE_SERVICES
public class GoogleDriveBackupProvider : IBackupProvider
{
    private const string DriveFilesUrl = "https://www.googleapis.com/drive/v3/files";
    private const string DriveUploadUrl =
        "https://www.googleapis.com/upload/drive/v3/files";
    private const string FolderMimeType = "application/vnd.google-apps.folder";
    private const string FolderName = "LifeChart";

    private readonly HttpClient _httpClient;
    private readonly ISettingsService _settings;

    public GoogleDriveBackupProvider(HttpClient httpClient, ISettingsService settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task BackupAsync(string dbPath)
    {
        var token = GetAccessToken();
        var folderId = await EnsureFolderExistsAsync(token);
        var fileName = $"lifechart_backup_{DateTime.Today:yyyy-MM-dd}.sqlite";

        // Vorhandenes gleichnamiges Backup suchen und löschen (kein Duplikat)
        var existing = await FindFileAsync(token, fileName, folderId);
        if (existing is not null)
            await DeleteFileAsync(token, existing);

        await UploadFileAsync(token, dbPath, fileName, folderId);
    }

    public async Task<IEnumerable<BackupInfo>> ListBackupsAsync()
    {
        var token = GetAccessToken();
        var folderId = await EnsureFolderExistsAsync(token);

        var query = Uri.EscapeDataString(
            $"'{folderId}' in parents and name contains 'lifechart_backup' " +
            "and trashed = false");

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{DriveFilesUrl}?q={query}&fields=files(id,name,size,modifiedTime)");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DriveFileList>();
        return result?.Files?.Select(f => new BackupInfo(
            f.Name ?? string.Empty,
            f.ModifiedTime ?? DateTime.MinValue,
            long.TryParse(f.Size, out var s) ? s : 0))
            .OrderByDescending(b => b.CreatedAt)
            ?? [];
    }

    public async Task RestoreAsync(BackupInfo backup, string targetPath)
    {
        var token = GetAccessToken();
        var file = await FindFileAsync(token, backup.FileName, null);
        if (file is null)
            throw new InvalidOperationException(
                $"Backup '{backup.FileName}' nicht in Google Drive gefunden.");

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{DriveFilesUrl}/{file}?alt=media");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync();
        await File.WriteAllBytesAsync(targetPath, bytes);
    }

    private string GetAccessToken()
    {
        var token = _settings.Load().GoogleDriveAccessToken;
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException(
                "Google Drive ist nicht autorisiert. Bitte in den Einstellungen anmelden.");
        return token;
    }

    private async Task<string> EnsureFolderExistsAsync(string token)
    {
        var query = Uri.EscapeDataString(
            $"name='{FolderName}' and mimeType='{FolderMimeType}' and trashed=false");
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{DriveFilesUrl}?q={query}&fields=files(id)");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DriveFileList>();
        if (result?.Files?.Count > 0)
            return result.Files[0].Id!;

        // Ordner anlegen
        var createRequest = new HttpRequestMessage(HttpMethod.Post, DriveFilesUrl)
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) },
            Content = JsonContent.Create(new { name = FolderName, mimeType = FolderMimeType })
        };
        var createResponse = await _httpClient.SendAsync(createRequest);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<DriveFile>();
        return created!.Id!;
    }

    private async Task<string?> FindFileAsync(string token, string name, string? folderId)
    {
        var q = folderId is not null
            ? $"name='{name}' and '{folderId}' in parents and trashed=false"
            : $"name='{name}' and trashed=false";

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{DriveFilesUrl}?q={Uri.EscapeDataString(q)}&fields=files(id)");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<DriveFileList>();
        return result?.Files?.FirstOrDefault()?.Id;
    }

    private async Task DeleteFileAsync(string token, string fileId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{DriveFilesUrl}/{fileId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await _httpClient.SendAsync(request);
    }

    private async Task UploadFileAsync(
        string token, string dbPath, string fileName, string folderId)
    {
        var fileBytes = await File.ReadAllBytesAsync(dbPath);
        var metadata = System.Text.Json.JsonSerializer.Serialize(new
        {
            name = fileName,
            parents = new[] { folderId }
        });

        var content = new MultipartContent("related");
        content.Add(new StringContent(metadata, System.Text.Encoding.UTF8,
            "application/json"));
        content.Add(new ByteArrayContent(fileBytes)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("application/octet-stream") }
        });

        var request = new HttpRequestMessage(HttpMethod.Post,
            $"{DriveUploadUrl}?uploadType=multipart")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) },
            Content = content
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private sealed class DriveFileList
    {
        [JsonPropertyName("files")]
        public List<DriveFile>? Files { get; set; }
    }

    private sealed class DriveFile
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("size")] public string? Size { get; set; }
        [JsonPropertyName("modifiedTime")] public DateTime? ModifiedTime { get; set; }
    }
}
#endif
