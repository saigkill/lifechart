using LifeChart.Application.Localization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LifeChart.Linux.Pages;

public class AboutPage : ContentPage
{
    public AboutPage()
    {
        Title = L.About_Title;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 16,
                Padding = new Thickness(32, 24),
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label
                    {
                        Text = "LifeChart",
                        FontSize = 28,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                    },
                    new Label
                    {
                        Text = L.About_Version,
                        FontSize = 13,
                        TextColor = Colors.Gray,
                        HorizontalOptions = LayoutOptions.Center,
                    },

                    new BoxView { HeightRequest = 4 },

                    new Label
                    {
                        Text = L.About_Description,
                        FontSize = 15,
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        MaximumWidthRequest = 480,
                    },

                    new BoxView { HeightRequest = 4 },

                    MakeSectionLabel(L.About_Developer),
                    MakeInfoLabel("Sascha Manns"),

                    MakeSectionLabel(L.About_License),
                    MakeInfoLabel("GNU General Public License v3.0"),

                    MakeSectionLabel(L.About_Copyright),
                    MakeInfoLabel("© 2026 Sascha Manns"),

                    new BoxView { HeightRequest = 8 },

                    MakeLinkButton(L.About_Wiki,
                        "https://gitlab.gnome.org/saigkill/lifechart/-/wikis/home"),
                    MakeLinkButton(L.About_Issues,
                        "https://gitlab.gnome.org/saigkill/lifechart/-/work_items"),

                    new BoxView { HeightRequest = 8 },

                    new Label
                    {
                        Text = L.About_ExperimentalNote,
                        FontSize = 12,
                        TextColor = Colors.OrangeRed,
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        MaximumWidthRequest = 480,
                    },
                }
            }
        };
    }

    private static Label MakeSectionLabel(string text) => new()
    {
        Text = text,
        FontSize = 12,
        TextColor = Colors.Gray,
        FontAttributes = FontAttributes.Bold,
        HorizontalOptions = LayoutOptions.Center,
    };

    private static Label MakeInfoLabel(string text) => new()
    {
        Text = text,
        FontSize = 15,
        HorizontalOptions = LayoutOptions.Center,
    };

    private static Button MakeLinkButton(string label, string url)
    {
        var btn = new Button
        {
            Text = label,
            HorizontalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Transparent,
            TextColor = Colors.CornflowerBlue,
            Padding = new Thickness(0),
        };
        btn.Clicked += async (_, _) =>
        {
            try { await Launcher.OpenAsync(new Uri(url)); }
            catch { /* Browser nicht verfügbar */ }
        };
        return btn;
    }
}
