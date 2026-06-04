using LifeChart.ViewModels;

namespace LifeChart.Pages;

public partial class TodayPage : ContentPage
{
    private readonly TodayViewModel _viewModel;

    public TodayPage(TodayViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        viewModel.CrisisHintRequested += OnCrisisHintRequested;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }

    private async void OnCrisisHintRequested(object? sender, EventArgs e)
    {
        var settings = ((App)Application.Current!).Settings;
        if (!settings.CrisisHintEnabled) return;

        bool navigateToCrisis = await DisplayAlert(
            "Schwerer Tag",
            "Es klingt nach einem sehr schweren Tag. Falls du mit jemandem sprechen möchtest, findest du hier Krisentelefone.",
            "Zur Hilfe",
            "Schließen");

        if (navigateToCrisis)
            await Shell.Current.GoToAsync("//CrisisPage");
    }
}
