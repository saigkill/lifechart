using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeChart.Application.DTOs;
using LifeChart.Application.UseCases.Medications;
using LifeChart.Services;
using System.Collections.ObjectModel;

namespace LifeChart.ViewModels;

public partial class MedicationsViewModel : ObservableObject
{
    private readonly GetActiveMedicationsUseCase _getMedications;
    private readonly DeactivateMedicationUseCase _deactivate;
    private readonly MedicationFormService _formService;

    [ObservableProperty] private ObservableCollection<MedicationDto> _medications = [];
    [ObservableProperty] private bool _isLoading;

    public MedicationsViewModel(
        GetActiveMedicationsUseCase getMedications,
        DeactivateMedicationUseCase deactivate,
        MedicationFormService formService)
    {
        _getMedications = getMedications;
        _deactivate = deactivate;
        _formService = formService;
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
    private async Task AddAsync()
    {
        _formService.EditTarget = null;
        await NavigateToForm();
    }

    [RelayCommand]
    private async Task EditAsync(MedicationDto medication)
    {
        _formService.EditTarget = medication;
        await NavigateToForm();
    }

    [RelayCommand]
    private async Task DeleteAsync(MedicationDto medication)
    {
        bool confirmed = await Shell.Current.DisplayAlert(
            "Medikament entfernen",
            $"'{medication.Name}' wirklich entfernen?",
            "Entfernen", "Abbrechen");

        if (!confirmed) return;

        await _deactivate.ExecuteAsync(medication.Id);
        Medications.Remove(medication);
    }

    private static async Task NavigateToForm()
        => await Shell.Current.GoToAsync("MedicationFormPage");
}
