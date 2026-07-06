using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeChart.Application.DTOs;
using LifeChart.Application.Localization;
using LifeChart.Application.UseCases.Crisis;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace LifeChart.ViewModels;

public sealed class RegionItem(string code, string displayName)
{
    public string Code { get; } = code;
    public string DisplayName { get; } = displayName;
    public override string ToString() => DisplayName;
}

public partial class CrisisViewModel : ObservableObject
{
    private readonly GetCrisisResourcesUseCase _getCrisisResources;

    [ObservableProperty] private ObservableCollection<CrisisResourceDto> _resources = [];
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private RegionItem? _selectedRegion;

    public RegionItem[] Regions { get; } =
    [
        new("DE", "Deutschland"),
        new("AT", "Österreich"),
        new("CH", "Schweiz"),
        new("GB", "United Kingdom"),
        new("US", "United States"),
        new("AU", "Australia"),
        new("CA", "Canada"),
    ];

    public CrisisViewModel(GetCrisisResourcesUseCase getCrisisResources)
    {
        _getCrisisResources = getCrisisResources;

        var detected = DetectRegionCode();
        _selectedRegion = Regions.FirstOrDefault(r => r.Code == detected) ?? Regions[0];
    }

    partial void OnSelectedRegionChanged(RegionItem? value) => _ = LoadResourcesAsync();

    public async Task InitializeAsync()
    {
        if (Resources.Count == 0)
            await LoadResourcesAsync();
    }

    private async Task LoadResourcesAsync()
    {
        if (SelectedRegion is null) return;

        IsLoading = true;
        ErrorMessage = null;
        IsEmpty = false;
        Resources.Clear();

        try
        {
            var resources = await _getCrisisResources.ExecuteAsync(SelectedRegion.Code);
            foreach (var r in resources)
                Resources.Add(r);
            IsEmpty = resources.Count() == 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = L.Fmt("Crisis_LoadErrorFmt", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CallAsync(string phoneNumber)
        => await Launcher.OpenAsync($"tel:{phoneNumber}");

    [RelayCommand]
    private async Task OpenWebAsync(string url)
        => await Launcher.OpenAsync(url);

    private static string DetectRegionCode()
    {
        try { return RegionInfo.CurrentRegion.TwoLetterISORegionName.ToUpperInvariant(); }
        catch { return "DE"; }
    }
}
