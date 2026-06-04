using System.Net.Http.Json;
using System.Text.Json.Serialization;
using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;

namespace LifeChart.Infrastructure.CrisisResources;

// API-Dokumentation: https://findahelpline.com
// Endpunkt wird bei Bedarf angepasst wenn offizielle Docs verfügbar sind.
public class FindAHelplineService : ICrisisResourceService
{
    private readonly HttpClient _httpClient;

    public FindAHelplineService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://findahelpline.com");
        _httpClient.Timeout = TimeSpan.FromSeconds(5);
    }

    public async Task<IEnumerable<CrisisResourceDto>> GetForRegionAsync(string regionCode)
    {
        var response = await _httpClient.GetFromJsonAsync<FindAHelplineResponse>(
            $"/api/v1/organizations?countryCode={regionCode}");

        if (response?.Organizations is null)
            return [];

        return response.Organizations.Select(o => new CrisisResourceDto(
            o.Name ?? string.Empty,
            o.PhoneNumber ?? string.Empty,
            o.Url,
            o.Available24h));
    }

    private sealed class FindAHelplineResponse
    {
        [JsonPropertyName("organizations")]
        public List<FindAHelplineOrganization>? Organizations { get; set; }
    }

    private sealed class FindAHelplineOrganization
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("phone")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("available_24h")]
        public bool Available24h { get; set; }
    }
}
