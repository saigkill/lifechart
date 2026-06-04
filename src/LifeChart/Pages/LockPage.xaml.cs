using LifeChart.ViewModels;

namespace LifeChart.Pages;

public partial class LockPage : ContentPage
{
    public LockPage(LockViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        viewModel.AuthenticationSucceeded += (_, _) =>
        {
            Application.Current!.MainPage = IPlatformApplication.Current!
                .Services.GetRequiredService<AppShell>();
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((LockViewModel)BindingContext).TryAuthenticateAsync();
    }
}
