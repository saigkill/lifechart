using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeChart.Application.DTOs;
using LifeChart.Application.Settings;
using LifeChart.Application.UseCases.Entries;

namespace LifeChart.ViewModels;

public partial class TodayViewModel : ObservableObject
{
    private readonly SaveDailyEntryUseCase _saveEntry;
    private readonly GetTodayEntryUseCase _getTodayEntry;
    private readonly ISettingsService _settings;

    [ObservableProperty] private int _moodValue;
    [ObservableProperty] private int _functionalityValue;
    [ObservableProperty] private int _sleepHours;
    [ObservableProperty] private bool _medicationTaken;
    [ObservableProperty] private bool _menstrualCycle;
    [ObservableProperty] private string? _symptoms;
    [ObservableProperty] private string? _notes;
    [ObservableProperty] private bool _isHypomanic;
    [ObservableProperty] private bool _showFullEntry;
    [ObservableProperty] private bool _entryAlreadySaved;

    public TodayViewModel(
        SaveDailyEntryUseCase saveEntry,
        GetTodayEntryUseCase getTodayEntry,
        ISettingsService settings)
    {
        _saveEntry = saveEntry;
        _getTodayEntry = getTodayEntry;
        _settings = settings;
    }

    public async Task InitializeAsync()
    {
        var appSettings = _settings.Load();
        ShowFullEntry = appSettings.InputMode == InputMode.Full;

        var existing = await _getTodayEntry.ExecuteAsync();
        if (existing is null) return;

        MoodValue = existing.Mood;
        FunctionalityValue = existing.Functionality;
        SleepHours = existing.SleepHours;
        MedicationTaken = existing.MedicationTaken;
        MenstrualCycle = existing.MenstrualCycle;
        Symptoms = existing.Symptoms;
        Notes = existing.Notes;
        IsHypomanic = existing.IsHypomanic;
        EntryAlreadySaved = true;
    }

    [RelayCommand]
    private void ToggleFullEntry() => ShowFullEntry = !ShowFullEntry;

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new DailyEntryDto(
            DateOnly.FromDateTime(DateTime.Today),
            MoodValue,
            FunctionalityValue,
            SleepHours,
            MedicationTaken,
            MenstrualCycle,
            Symptoms,
            Notes,
            IsHypomanic);

        var result = await _saveEntry.ExecuteAsync(dto);
        EntryAlreadySaved = true;

        if (result.IsCritical)
            CrisisHintRequested?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? CrisisHintRequested;
}
