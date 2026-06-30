using LifeChart.Application.DTOs;
using LifeChart.Application.Localization;
using LifeChart.Application.UseCases.Crisis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Globalization;

namespace LifeChart.Linux.Pages;

public partial class CrisisPage : ContentPage
{
    private static readonly (string Code, string Label)[] Regions =
    [
        ("DE", "Deutschland"),
        ("AT", "Österreich"),
        ("CH", "Schweiz"),
        ("GB", "United Kingdom"),
        ("US", "United States"),
        ("AU", "Australia"),
        ("CA", "Canada"),
    ];

    public CrisisPage()
    {
        InitializeComponent();
        Title = L.Crisis_Title;
        NotAloneLabel.Text = L.Crisis_NotAlone;
        RegionLabel.Text = L.Crisis_Region;

        foreach (var (_, label) in Regions)
            RegionPicker.Items.Add(label);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var regionCode = DetectRegionCode();
        var idx = Array.FindIndex(Regions, r => r.Code == regionCode);
        RegionPicker.SelectedIndex = idx >= 0 ? idx : 0;

        if (ResourcesLayout.Children.Count == 0)
            await LoadResourcesAsync();
    }

    private async void OnRegionPickerChanged(object? sender, EventArgs e)
        => await LoadResourcesAsync();

    private async Task LoadResourcesAsync()
    {
        if (RegionPicker.SelectedIndex < 0) return;

        Spinner.IsRunning = true;
        Spinner.IsVisible = true;
        ErrorLabel.IsVisible = false;
        ResourcesLayout.Children.Clear();

        try
        {
            var regionCode = Regions[RegionPicker.SelectedIndex].Code;
            var useCase = IPlatformApplication.Current!.Services
                .GetRequiredService<GetCrisisResourcesUseCase>();
            var resources = (await useCase.ExecuteAsync(regionCode)).ToList();

            foreach (var r in resources)
                ResourcesLayout.Children.Add(BuildResourceCard(r));

            if (resources.Count == 0)
            {
                ErrorLabel.Text = L.Crisis_Empty;
                ErrorLabel.TextColor = Colors.Gray;
                ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = L.Fmt("Crisis_LoadErrorFmt", ex.Message);
            ErrorLabel.IsVisible = true;
        }
        finally
        {
            Spinner.IsRunning = false;
            Spinner.IsVisible = false;
        }
    }

    private static View BuildResourceCard(CrisisResourceDto r)
    {
        var callButton = new Button
        {
            Text = L.Crisis_Call,
            BackgroundColor = Color.FromArgb("#0072B2"),
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Start,
        };
        callButton.Clicked += async (_, _) =>
        {
            try { await Launcher.OpenAsync($"tel:{r.PhoneNumber}"); }
            catch { await Clipboard.SetTextAsync(r.PhoneNumber); }
        };

        var cardLayout = new VerticalStackLayout { Spacing = 8 };
        cardLayout.Children.Add(new Label { Text = r.Name, FontSize = 16, FontAttributes = FontAttributes.Bold });
        cardLayout.Children.Add(new Label { Text = r.PhoneNumber, FontSize = 20, TextColor = Color.FromArgb("#0072B2") });

        if (r.IsAvailable24h)
            cardLayout.Children.Add(new Label { Text = L.Crisis_Available24h, TextColor = Colors.Gray, FontSize = 12 });

        cardLayout.Children.Add(callButton);

        if (!string.IsNullOrEmpty(r.Url))
        {
            var webButton = new Button
            {
                Text = L.Crisis_Website,
                BackgroundColor = Colors.Transparent,
                TextColor = Color.FromArgb("#0072B2"),
                HorizontalOptions = LayoutOptions.Start,
                Padding = new Thickness(0),
            };
            webButton.Clicked += async (_, _) =>
            {
                try { await Launcher.OpenAsync(r.Url!); }
                catch { /* Browser nicht verfügbar */ }
            };
            cardLayout.Children.Add(webButton);
        }

        return new Frame
        {
            Padding = new Thickness(16),
            CornerRadius = 8,
            Content = cardLayout,
        };
    }

    private static string DetectRegionCode()
    {
        try { return RegionInfo.CurrentRegion.TwoLetterISORegionName.ToUpperInvariant(); }
        catch { return "DE"; }
    }
}
