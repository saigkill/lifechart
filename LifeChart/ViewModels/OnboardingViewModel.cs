using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeChart.Application.Interfaces;
using LifeChart.Application.Settings;

namespace LifeChart.ViewModels;

public partial class OnboardingViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IAlarmService _alarmService;

    [ObservableProperty] private TimeOnly _reminderTime = new(20, 0);
    [ObservableProperty] private bool _reminderEnabled = true;
    [ObservableProperty] private bool _disclaimerAccepted;

    public OnboardingViewModel(ISettingsService settingsService, IAlarmService alarmService)
    {
        _settingsService = settingsService;
        _alarmService = alarmService;
    }

    [RelayCommand]
    private async Task CompleteAsync()
    {
        // Permissions anfordern bevor Alarme eingerichtet werden
        if (ReminderEnabled)
            await _alarmService.RequestPermissionsAsync();

        var settings = _settingsService.Load();
        settings.EveningReminderEnabled = ReminderEnabled;
        settings.EveningReminderTime = ReminderEnabled ? ReminderTime : null;
        await _settingsService.SaveAsync(settings);

        OnboardingCompleted?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? OnboardingCompleted;
}
