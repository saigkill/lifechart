using CommunityToolkit.Mvvm.ComponentModel;

namespace LifeChart.ViewModels;

public partial class IntakeTimeEntryViewModel : ObservableObject
{
    [ObservableProperty] private TimeSpan _time = new(8, 0, 0);
    [ObservableProperty] private int _doseCount = 1;

    public TimeOnly ToTimeOnly() => TimeOnly.FromTimeSpan(Time);
}
