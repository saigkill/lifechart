using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeChart.Application.DTOs;
using LifeChart.Application.UseCases.Crisis;
using System.Collections.ObjectModel;
using System.Globalization;

namespace LifeChart.ViewModels;

public partial class CrisisViewModel : ObservableObject
{
    private readonly GetCrisisResourcesUseCase _getCrisisResources;

    [ObservableProperty] private ObservableCollection<CrisisResourceDto> _resources = [];
    [ObservableProperty] private bool _isLoading;

    public CrisisViewModel(GetCrisisResourcesUseCase getCrisisResources)
        => _getCrisisResources = getCrisisResources;

    public async Task InitializeAsync()
    {
        IsLoading = true;
        try
        {
            var regionCode = RegionInfo.CurrentRegion.TwoLetterISORegionName;
            var resources = await _getCrisisResources.ExecuteAsync(regionCode);
            Resources = new ObservableCollection<CrisisResourceDto>(resources);
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
}
