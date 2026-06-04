using LifeChart.Application.DTOs;
using LifeChart.Application.Localization;
using LifeChart.Application.UseCases.Crisis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Globalization;

namespace LifeChart.Linux.Pages;

public class CrisisPage : ContentPage
{
    private readonly Picker _regionPicker = new() { HorizontalOptions = LayoutOptions.FillAndExpand };
    private readonly ActivityIndicator _spinner = new() { IsRunning = false, IsVisible = false, HorizontalOptions = LayoutOptions.Center };
    private readonly VerticalStackLayout _resourcesLayout = new() { Spacing = 10 };
    private readonly Label _errorLabel = new() { TextColor = Colors.Red, HorizontalOptions = LayoutOptions.Center, IsVisible = false };

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
        Title = L.Crisis_Title;

        foreach (var (_, label) in Regions)
            _regionPicker.Items.Add(label);

        _regionPicker.SelectedIndexChanged += async (_, _) => await LoadResourcesAsync();

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 16,
                Padding = new Thickness(24, 16),
                Children =
                {
                    new Label
                    {
                        Text = L.Crisis_NotAlone,
                        FontSize = 18,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Center,
                    },
                    new HorizontalStackLayout
                    {
                        Spacing = 12,
                        Children =
                        {
                            new Label { Text = L.Crisis_Region, VerticalOptions = LayoutOptions.Center },
                            _regionPicker,
                        }
                    },
                    _spinner,
                    _errorLabel,
                    _resourcesLayout,
                }
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Region aus Systemlocale ermitteln, Fallback auf DE
        var regionCode = DetectRegionCode();
        var idx = Array.FindIndex(Regions, r => r.Code == regionCode);
        _regionPicker.SelectedIndex = idx >= 0 ? idx : 0;

        // SelectedIndexChanged feuert bereits — nur laden falls nicht schon geschehen
        if (_resourcesLayout.Children.Count == 0)
            await LoadResourcesAsync();
    }

    private async Task LoadResourcesAsync()
    {
        if (_regionPicker.SelectedIndex < 0) return;

        _spinner.IsRunning = true;
        _spinner.IsVisible = true;
        _errorLabel.IsVisible = false;
        _resourcesLayout.Children.Clear();

        try
        {
            var regionCode = Regions[_regionPicker.SelectedIndex].Code;
            var useCase = IPlatformApplication.Current!.Services
                .GetRequiredService<GetCrisisResourcesUseCase>();
            var resources = (await useCase.ExecuteAsync(regionCode)).ToList();

            foreach (var r in resources)
                _resourcesLayout.Children.Add(BuildResourceCard(r));

            if (resources.Count == 0)
            {
                _errorLabel.Text = L.Crisis_Empty;
                _errorLabel.TextColor = Colors.Gray;
                _errorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            _errorLabel.Text = L.Fmt("Crisis_LoadErrorFmt", ex.Message);
            _errorLabel.IsVisible = true;
        }
        finally
        {
            _spinner.IsRunning = false;
            _spinner.IsVisible = false;
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

        var children = new List<IView>
        {
            new Label { Text = r.Name, FontSize = 16, FontAttributes = FontAttributes.Bold },
            new Label { Text = r.PhoneNumber, FontSize = 20, TextColor = Color.FromArgb("#0072B2") },
        };

        if (r.IsAvailable24h)
            children.Add(new Label { Text = L.Crisis_Available24h, TextColor = Colors.Gray, FontSize = 12 });

        children.Add(callButton);

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
            children.Add(webButton);
        }

        var card = new Frame
        {
            Padding = new Thickness(16),
            CornerRadius = 8,
            Content = new VerticalStackLayout { Spacing = 8 }
        };

        foreach (var child in children)
            ((VerticalStackLayout)card.Content).Children.Add((View)child);

        return card;
    }

    private static string DetectRegionCode()
    {
        try
        {
            return RegionInfo.CurrentRegion.TwoLetterISORegionName.ToUpperInvariant();
        }
        catch
        {
            return "DE";
        }
    }
}
