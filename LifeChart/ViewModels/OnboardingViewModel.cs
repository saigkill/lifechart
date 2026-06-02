using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeChart.Application.Settings;

namespace LifeChart.ViewModels;

public partial class OnboardingViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty] private TimeOnly _reminderTime = new(20, 0);
    [ObservableProperty] private bool _reminderEnabled = true;
    [ObservableProperty] private bool _disclaimerAccepted;

    public OnboardingViewModel(ISettingsService settingsService)
        => _settingsService = settingsService;

    [RelayCommand]
    private async Task CompleteAsync()
    {
        var settings = _settingsService.Load();
        settings.EveningReminderEnabled = ReminderEnabled;
        settings.EveningReminderTime = ReminderEnabled ? ReminderTime : null;
        await _settingsService.SaveAsync(settings);

        OnboardingCompleted?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? OnboardingCompleted;
}
