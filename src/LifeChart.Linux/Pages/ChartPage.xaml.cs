using LifeChart.Application.Localization;
using LifeChart.Application.UseCases.Entries;
using LifeChart.Application.UseCases.Pdf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LifeChart.Linux.Pages;

public partial class ChartPage : ContentPage
{
    private readonly MoodChartDrawable _drawable = new();
    private int _days = 30;

    public ChartPage()
    {
        InitializeComponent();
        Title = L.Chart_Title;
        HeaderLabel.Text = L.Chart_Header;
        PdfButton.Text = L.Chart_ExportPdf;
        EmptyLabel.Text = L.Chart_Empty;
        MoodLegendLabel.Text = L.Today_Mood;
        FuncLegendLabel.Text = L.Today_Functionality;
        HypomaniaLegendLabel.Text = L.Chart_Hypomania;

        ChartView.Drawable = _drawable;

        BuildRangeButtons();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadChartAsync();
    }

    private void BuildRangeButtons()
    {
        RangeButtonRow.Children.Clear();
        foreach (var (label, days) in new[] {
            (L.Chart_Days30, 30),
            (L.Chart_Days60, 60),
            (L.Chart_Days90, 90) })
        {
            var btn = new Button
            {
                Text = label,
                BackgroundColor = days == _days ? Color.FromArgb("#0072B2") : Colors.LightGray,
                TextColor = days == _days ? Colors.White : Colors.Black,
                Padding = new Thickness(12, 6),
                CornerRadius = 16,
            };
            var capturedDays = days;
            btn.Clicked += async (_, _) =>
            {
                _days = capturedDays;
                BuildRangeButtons();
                await LoadChartAsync();
            };
            RangeButtonRow.Children.Add(btn);
        }
    }

    private async Task LoadChartAsync()
    {
        var to = DateOnly.FromDateTime(DateTime.Today);
        var from = to.AddDays(-_days);

        var data = await IPlatformApplication.Current!.Services
            .GetRequiredService<GetChartDataUseCase>()
            .ExecuteAsync(from, to);

        _drawable.Data = data;
        ChartView.Invalidate();

        EmptyLabel.IsVisible = data.Points.Count == 0;
        ChartView.IsVisible = data.Points.Count > 0;
    }

    private async void OnExportPdfClicked(object? sender, EventArgs e)
    {
        try
        {
            var to = DateOnly.FromDateTime(DateTime.Today);
            var from = to.AddDays(-_days);

            var pdfBytes = await IPlatformApplication.Current!.Services
                .GetRequiredService<ExportPdfUseCase>()
                .ExecuteAsync(from, to);

            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "LifeChart");
            Directory.CreateDirectory(dir);

            var fileName = $"LifeChart_{from:yyyy-MM-dd}_{to:yyyy-MM-dd}.pdf";
            var path = Path.Combine(dir, fileName);
            await File.WriteAllBytesAsync(path, pdfBytes);

            var open = await DisplayAlert(
                L.Chart_PdfExported,
                L.Fmt("Chart_PdfSavedFmt", "\n", path),
                L.Chart_OpenFile, L.Common_OK);

            if (open)
            {
                try { await Launcher.OpenAsync(new Uri($"file://{path}")); }
                catch { /* kein PDF-Viewer installiert */ }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(L.Common_Error, ex.Message, L.Common_OK);
        }
    }
}
