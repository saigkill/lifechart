using LifeChart.ViewModels;
using MauiApplication = Microsoft.Maui.Controls.Application;

namespace LifeChart.Pages;

public partial class OnboardingPage : ContentPage
{
    public OnboardingPage(OnboardingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        viewModel.OnboardingCompleted += async (_, _) =>
        {
            MauiApplication.Current!.MainPage = new AppShell();
        };
    }
}
