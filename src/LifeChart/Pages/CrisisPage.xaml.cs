using LifeChart.ViewModels;

namespace LifeChart.Pages;

public partial class CrisisPage : ContentPage
{
    private readonly CrisisViewModel _viewModel;

    public CrisisPage(CrisisViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
