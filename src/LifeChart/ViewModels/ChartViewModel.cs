using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeChart.Application.DTOs;
using LifeChart.Application.UseCases.Entries;
using LifeChart.Application.UseCases.Pdf;

namespace LifeChart.ViewModels;

public partial class ChartViewModel : ObservableObject
{
    private readonly GetChartDataUseCase _getChartData;
    private readonly ExportPdfUseCase _exportPdf;

    [ObservableProperty] private ChartDataDto? _chartData;
    [ObservableProperty] private int _selectedPeriodDays = 30;
    [ObservableProperty] private bool _isExporting;

    public int[] AvailablePeriods { get; } = [30, 60, 90];

    public ChartViewModel(GetChartDataUseCase getChartData, ExportPdfUseCase exportPdf)
    {
        _getChartData = getChartData;
        _exportPdf = exportPdf;
    }

    public async Task InitializeAsync() => await LoadChartAsync();

    [RelayCommand]
    private async Task ChangePeriodAsync(string? days)
    {
        if (!int.TryParse(days, out var parsed))
            return;

        SelectedPeriodDays = parsed;
        await LoadChartAsync();
    }

    [RelayCommand]
    private async Task ExportPdfAsync()
    {
        IsExporting = true;
        try
        {
            var to = DateOnly.FromDateTime(DateTime.Today);
            var from = to.AddDays(-SelectedPeriodDays);
            var pdfBytes = await _exportPdf.ExecuteAsync(from, to);
            PdfExportReady?.Invoke(this, pdfBytes);
        }
        finally
        {
            IsExporting = false;
        }
    }

    private async Task LoadChartAsync()
    {
        var to = DateOnly.FromDateTime(DateTime.Today);
        var from = to.AddDays(-SelectedPeriodDays);
        ChartData = await _getChartData.ExecuteAsync(from, to);
    }

    public event EventHandler<byte[]>? PdfExportReady;
}
