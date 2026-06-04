using LifeChart.ViewModels;

namespace LifeChart.Pages;

public partial class MedicationFormPage : ContentPage
{
    private readonly MedicationFormViewModel _viewModel;

    public MedicationFormPage(MedicationFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        viewModel.SaveCompleted += async (_, _) =>
            await Navigation.PopModalAsync();

        viewModel.CancelRequested += async (_, _) =>
            await Navigation.PopModalAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Initialize();
    }
}
