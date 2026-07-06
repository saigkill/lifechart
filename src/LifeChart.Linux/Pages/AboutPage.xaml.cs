using LifeChart.Application.Localization;
using Microsoft.Maui.Controls;

namespace LifeChart.Linux.Pages;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
        Title = L.About_Title;
        VersionLabel.Text = L.About_Version;
        DescriptionLabel.Text = L.About_Description;
        DeveloperSectionLabel.Text = L.About_Developer;
        LicenseSectionLabel.Text = L.About_License;
        CopyrightSectionLabel.Text = L.About_Copyright;
        WikiButton.Text = L.About_Wiki;
        IssuesButton.Text = L.About_Issues;
        //ExperimentalLabel.Text = L.About_ExperimentalNote;
    }

    private async void OnWikiClicked(object? sender, EventArgs e)
    {
        try { await Launcher.OpenAsync(new Uri("https://writebook.saschamanns.de/6/lifechart")); }
        catch { /* Browser nicht verfügbar */ }
    }

    private async void OnIssuesClicked(object? sender, EventArgs e)
    {
        try { await Launcher.OpenAsync(new Uri("https://github.com/saigkill/lifechart/issues")); }
        catch { /* Browser nicht verfügbar */ }
    }
}
