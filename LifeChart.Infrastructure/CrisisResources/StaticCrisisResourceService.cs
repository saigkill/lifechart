using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;

namespace LifeChart.Infrastructure.CrisisResources;

public class StaticCrisisResourceService : ICrisisResourceService
{
    private static readonly Dictionary<string, List<CrisisResourceDto>> Resources = new()
    {
        ["DE"] =
        [
            new("Telefonseelsorge", "0800 111 0 111", "https://www.telefonseelsorge.de", true),
            new("Telefonseelsorge", "0800 111 0 222", "https://www.telefonseelsorge.de", true)
        ],
        ["AT"] =
        [
            new("Telefonseelsorge Österreich", "142", "https://www.telefonseelsorge.at", true)
        ],
        ["CH"] =
        [
            new("Die Dargebotene Hand", "143", "https://www.143.ch", true)
        ],
        ["GB"] =
        [
            new("Samaritans", "116 123", "https://www.samaritans.org", true)
        ],
        ["US"] =
        [
            new("988 Suicide & Crisis Lifeline", "988", "https://988lifeline.org", true)
        ],
        ["AU"] =
        [
            new("Lifeline Australia", "13 11 14", "https://www.lifeline.org.au", true)
        ],
        ["CA"] =
        [
            new("Crisis Services Canada", "1-833-456-4566", "https://www.crisisservicescanada.ca", true)
        ]
    };

    private static readonly List<CrisisResourceDto> FallbackResources =
    [
        new("findahelpline.com", "", "https://findahelpline.com", true)
    ];

    public Task<IEnumerable<CrisisResourceDto>> GetForRegionAsync(string regionCode)
    {
        var result = Resources.TryGetValue(regionCode.ToUpperInvariant(), out var resources)
            ? resources
            : FallbackResources;

        return Task.FromResult<IEnumerable<CrisisResourceDto>>(result);
    }
}
