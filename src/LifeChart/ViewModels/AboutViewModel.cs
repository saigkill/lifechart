using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LifeChart.ViewModels;

public partial class AboutViewModel : ObservableObject
{
    [RelayCommand]
    private async Task OpenWikiAsync()
    {
        try { await Launcher.OpenAsync(new Uri("https://writebook.saschamanns.de/6/lifechart")); }
        catch { }
    }

    [RelayCommand]
    private async Task OpenIssuesAsync()
    {
        try { await Launcher.OpenAsync(new Uri("https://github.com/saigkill/lifechart/issues")); }
        catch { }
    }
}
