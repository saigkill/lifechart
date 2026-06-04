using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using LifeChart.Application.Interfaces;
using LifeChart.Application.Settings;

namespace LifeChart.Infrastructure.Backup;

public class NextcloudBackupProvider : IBackupProvider
{
    private readonly HttpClient _httpClient;
    private readonly ISettingsService _settings;

    public NextcloudBackupProvider(HttpClient httpClient, ISettingsService settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task BackupAsync(string dbPath)
    {
        var (webDavUrl, auth) = GetConfig();
        await EnsureFolderExistsAsync(webDavUrl, auth);

        var fileName = $"lifechart_backup_{DateTime.Today:yyyy-MM-dd}.sqlite";
        var fileBytes = await File.ReadAllBytesAsync(dbPath);

        var request = new HttpRequestMessage(HttpMethod.Put, $"{webDavUrl}{fileName}")
        {
            Headers = { Authorization = auth },
            Content = new ByteArrayContent(fileBytes)
        };
        request.Content.Headers.ContentType =
            new MediaTypeHeaderValue("application/octet-stream");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<BackupInfo>> ListBackupsAsync()
    {
        var (webDavUrl, auth) = GetConfig();

        var request = new HttpRequestMessage(new HttpMethod("PROPFIND"), webDavUrl)
        {
            Content = new StringContent(PropfindBody, Encoding.UTF8, "application/xml")
        };
        request.Headers.Authorization = auth;
        request.Headers.Add("Depth", "1");

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) return [];

        var xml = await response.Content.ReadAsStringAsync();
        return ParsePropfindResponse(xml);
    }

    public async Task RestoreAsync(BackupInfo backup, string targetPath)
    {
        var (webDavUrl, auth) = GetConfig();

        var request = new HttpRequestMessage(HttpMethod.Get, $"{webDavUrl}{backup.FileName}")
        {
            Headers = { Authorization = auth }
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync();
        await File.WriteAllBytesAsync(targetPath, bytes);
    }

    private (string webDavUrl, AuthenticationHeaderValue auth) GetConfig()
    {
        var settings = _settings.Load();

        if (string.IsNullOrEmpty(settings.NextcloudUrl) ||
            string.IsNullOrEmpty(settings.NextcloudUsername) ||
            string.IsNullOrEmpty(settings.NextcloudPassword))
            throw new InvalidOperationException("Nextcloud ist nicht konfiguriert.");

        var baseUrl = settings.NextcloudUrl.TrimEnd('/');
        var webDavUrl =
            $"{baseUrl}/remote.php/dav/files/{settings.NextcloudUsername}/LifeChart/";

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(
                $"{settings.NextcloudUsername}:{settings.NextcloudPassword}"));
        var auth = new AuthenticationHeaderValue("Basic", credentials);

        return (webDavUrl, auth);
    }

    private async Task EnsureFolderExistsAsync(string webDavUrl, AuthenticationHeaderValue auth)
    {
        var request = new HttpRequestMessage(new HttpMethod("MKCOL"), webDavUrl)
        {
            Headers = { Authorization = auth }
        };
        // 201 Created or 405 Method Not Allowed (folder exists) — beide OK
        await _httpClient.SendAsync(request);
    }

    private static IEnumerable<BackupInfo> ParsePropfindResponse(string xml)
    {
        var doc = XDocument.Parse(xml);
        XNamespace dav = "DAV:";

        return doc.Descendants(dav + "response")
            .Where(r =>
            {
                var href = r.Element(dav + "href")?.Value ?? string.Empty;
                return href.EndsWith(".sqlite", StringComparison.OrdinalIgnoreCase);
            })
            .Select(r =>
            {
                var href = r.Element(dav + "href")!.Value;
                var fileName = Uri.UnescapeDataString(href.Split('/').Last());
                var lastModStr = r.Descendants(dav + "getlastmodified")
                    .FirstOrDefault()?.Value ?? string.Empty;
                DateTime.TryParse(lastModStr, out var lastMod);
                var sizeStr = r.Descendants(dav + "getcontentlength")
                    .FirstOrDefault()?.Value ?? "0";
                long.TryParse(sizeStr, out var size);
                return new BackupInfo(fileName, lastMod, size);
            })
            .OrderByDescending(b => b.CreatedAt);
    }

    private const string PropfindBody = """
        <?xml version="1.0" encoding="utf-8"?>
        <d:propfind xmlns:d="DAV:">
          <d:prop>
            <d:getlastmodified/>
            <d:getcontentlength/>
          </d:prop>
        </d:propfind>
        """;
}
