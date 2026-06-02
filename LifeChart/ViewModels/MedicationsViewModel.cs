using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeChart.Application.DTOs;
using LifeChart.Application.UseCases.Medications;
using System.Collections.ObjectModel;

namespace LifeChart.ViewModels;

public partial class MedicationsViewModel : ObservableObject
{
    private readonly GetActiveMedicationsUseCase _getMedications;
    private readonly DeactivateMedicationUseCase _deactivate;

    [ObservableProperty] private ObservableCollection<MedicationDto> _medications = [];
    [ObservableProperty] private bool _isLoading;

    public MedicationsViewModel(
        GetActiveMedicationsUseCase getMedications,
        DeactivateMedicationUseCase deactivate)
    {
        _getMedications = getMedications;
        _deactivate = deactivate;
    }

    public async Task InitializeAsync() => await LoadAsync();

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var medications = await _getMedications.ExecuteAsync();
            Medications = new ObservableCollection<MedicationDto>(medications);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(MedicationDto medication)
    {
        await _deactivate.ExecuteAsync(medication.Id);
        Medications.Remove(medication);
    }
}
