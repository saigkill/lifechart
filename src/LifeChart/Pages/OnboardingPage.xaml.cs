using LifeChart.ViewModels;

namespace LifeChart.Pages;

public partial class OnboardingPage : ContentPage
{
    public OnboardingPage(OnboardingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        viewModel.OnboardingCompleted += async (_, _) =>
        {
            Application.Current!.MainPage = new AppShell();
        };
    }
}
