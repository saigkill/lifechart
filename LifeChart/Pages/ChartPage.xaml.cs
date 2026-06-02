using LifeChart.ViewModels;

namespace LifeChart.Pages;

public partial class ChartPage : ContentPage
{
    private readonly ChartViewModel _viewModel;

    public ChartPage(ChartViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        viewModel.PdfExportReady += OnPdfExportReady;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }

    private async void OnPdfExportReady(object? sender, byte[] pdfBytes)
    {
        var fileName = $"lifechart_{DateTime.Today:yyyy-MM-dd}.pdf";
        var path = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllBytesAsync(path, pdfBytes);
        await Share.RequestAsync(new ShareFileRequest
        {
            Title = "LifeChart PDF exportieren",
            File = new ShareFile(path)
        });
    }
}
