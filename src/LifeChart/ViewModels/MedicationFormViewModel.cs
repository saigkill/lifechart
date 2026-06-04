using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;
using LifeChart.Application.UseCases.Medications;
using LifeChart.Services;
using System.Collections.ObjectModel;

namespace LifeChart.ViewModels;

public partial class MedicationFormViewModel : ObservableObject
{
    private readonly SaveMedicationUseCase _saveMedication;
    private readonly GetActiveMedicationsUseCase _getMedications;
    private readonly IAlarmService _alarmService;
    private readonly MedicationFormService _formService;

    [ObservableProperty] private int _medicationId;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _dosage = string.Empty;
    [ObservableProperty] private int _minStock;
    [ObservableProperty] private int _currentStock;
    [ObservableProperty] private bool _isEditMode;
    [ObservableProperty] private ObservableCollection<IntakeTimeEntryViewModel> _intakeTimes = [];

    public string Title => IsEditMode ? "Medikament bearbeiten" : "Neues Medikament";

    public MedicationFormViewModel(
        SaveMedicationUseCase saveMedication,
        GetActiveMedicationsUseCase getMedications,
        IAlarmService alarmService,
        MedicationFormService formService)
    {
        _saveMedication = saveMedication;
        _getMedications = getMedications;
        _alarmService = alarmService;
        _formService = formService;
    }

    public void Initialize()
    {
        var target = _formService.EditTarget;
        IsEditMode = target is not null;

        if (target is null)
        {
            MedicationId = 0;
            Name = string.Empty;
            Dosage = string.Empty;
            MinStock = 0;
            CurrentStock = 0;
            IntakeTimes = [new IntakeTimeEntryViewModel()];
            return;
        }

        MedicationId = target.Id;
        Name = target.Name;
        Dosage = target.Dosage;
        MinStock = target.MinStock;
        CurrentStock = target.CurrentStock;
        IntakeTimes = new ObservableCollection<IntakeTimeEntryViewModel>(
            target.IntakeTimes.Select(i => new IntakeTimeEntryViewModel
            {
                Time = TimeOnly.Parse(i.Time).ToTimeSpan(),
                DoseCount = i.DoseCount
            }));
    }

    [RelayCommand]
    private void AddIntakeTime()
        => IntakeTimes.Add(new IntakeTimeEntryViewModel());

    [RelayCommand]
    private void RemoveIntakeTime(IntakeTimeEntryViewModel item)
    {
        if (IntakeTimes.Count > 1)
            IntakeTimes.Remove(item);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name)) return;

        var dto = new SaveMedicationDto(
            Id: MedicationId,
            Name: Name.Trim(),
            Dosage: Dosage.Trim(),
            MinStock: MinStock,
            CurrentStock: CurrentStock,
            IntakeTimes: IntakeTimes
                .Select(i => new IntakeTimeDto(
                    i.ToTimeOnly().ToString("HH:mm"),
                    Math.Max(1, i.DoseCount)))
                .ToList()
                .AsReadOnly());

        await _saveMedication.ExecuteAsync(dto);

        // Alarme für alle aktiven Medikamente neu planen
        var allMedications = await _getMedications.ExecuteAsync();
        await _alarmService.ScheduleAsync(allMedications);

        _formService.EditTarget = null;
        SaveCompleted?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Cancel()
    {
        _formService.EditTarget = null;
        CancelRequested?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? SaveCompleted;
    public event EventHandler? CancelRequested;
}
