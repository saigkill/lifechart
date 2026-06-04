using LifeChart.Application.DTOs;
using LifeChart.Application.Localization;
using LifeChart.Application.UseCases.Entries;
using LifeChart.Application.UseCases.Pdf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LifeChart.Linux.Pages;

public class ChartPage : ContentPage
{
    private readonly MoodChartDrawable _drawable = new();
    private readonly GraphicsView _graphicsView = new() { HeightRequest = 300 };
    private readonly Label _emptyLabel = new()
    {
        HorizontalOptions = LayoutOptions.Center,
        TextColor = Colors.Gray,
        IsVisible = false,
    };

    private int _days = 30;

    public ChartPage()
    {
        Title = L.Chart_Title;
        _emptyLabel.Text = L.Chart_Empty;
        _graphicsView.Drawable = _drawable;

        var btn30 = MakeRangeButton(L.Chart_Days30, 30);
        var btn60 = MakeRangeButton(L.Chart_Days60, 60);
        var btn90 = MakeRangeButton(L.Chart_Days90, 90);

        var pdfButton = new Button
        {
            Text = L.Chart_ExportPdf,
            HorizontalOptions = LayoutOptions.End,
            BackgroundColor = Color.FromArgb("#0072B2"),
            TextColor = Colors.White,
            Padding = new Thickness(12, 6),
        };
        pdfButton.Clicked += OnExportPdfClicked;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Padding = new Thickness(16, 16),
                Children =
                {
                    new HorizontalStackLayout
                    {
                        Children =
                        {
                            new Label
                            {
                                Text = L.Chart_Header,
                                FontSize = 18,
                                FontAttributes = FontAttributes.Bold,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                            },
                            pdfButton,
                        }
                    },
                    new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children = { btn30, btn60, btn90 },
                    },
                    _graphicsView,
                    _emptyLabel,
                    BuildLegend(),
                }
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadChartAsync();
    }

    private async Task LoadChartAsync()
    {
        var to = DateOnly.FromDateTime(DateTime.Today);
        var from = to.AddDays(-_days);

        var data = await IPlatformApplication.Current!.Services
            .GetRequiredService<GetChartDataUseCase>()
            .ExecuteAsync(from, to);

        _drawable.Data = data;
        _graphicsView.Invalidate();

        _emptyLabel.IsVisible = data.Points.Count == 0;
        _graphicsView.IsVisible = data.Points.Count > 0;
    }

    private Button MakeRangeButton(string label, int days)
    {
        var btn = new Button
        {
            Text = label,
            BackgroundColor = days == _days
                ? Color.FromArgb("#0072B2")
                : Colors.LightGray,
            TextColor = days == _days ? Colors.White : Colors.Black,
            Padding = new Thickness(12, 6),
            CornerRadius = 16,
        };
        btn.Clicked += async (_, _) =>
        {
            _days = days;
            await LoadChartAsync();
        };
        return btn;
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

    private static View BuildLegend() =>
        new HorizontalStackLayout
        {
            Spacing = 24,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                MakeLegendItem(L.Today_Mood, Color.FromArgb("#0072B2")),
                MakeLegendItem(L.Today_Functionality, Color.FromArgb("#E69F00")),
                MakeLegendItem(L.Chart_Hypomania, Color.FromArgb("#CC79A7")),
            }
        };

    private static View MakeLegendItem(string label, Color color) =>
        new HorizontalStackLayout
        {
            Spacing = 6,
            Children =
            {
                new BoxView { Color = color, WidthRequest = 24, HeightRequest = 4,
                    VerticalOptions = LayoutOptions.Center },
                new Label { Text = label, FontSize = 13 },
            }
        };
}

public class MoodChartDrawable : IDrawable
{
    public ChartDataDto? Data { get; set; }

    private const float MarginLeft = 40f;
    private const float MarginRight = 16f;
    private const float MarginTop = 16f;
    private const float MarginBottom = 36f;
    private const int YMin = -5;
    private const int YMax = 5;

    private static readonly Color MoodColor = Color.FromArgb("#0072B2");
    private static readonly Color FuncColor = Color.FromArgb("#E69F00");
    private static readonly Color HypomaniaColor = Color.FromArgb("#CC79A7");
    private static readonly Color GridColor = Color.FromArgb("#E0E0E0");
    private static readonly Color AxisColor = Color.FromArgb("#888888");

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Data is null || Data.Points.Count == 0) return;

        var w = dirtyRect.Width;
        var h = dirtyRect.Height;
        var chartW = w - MarginLeft - MarginRight;
        var chartH = h - MarginTop - MarginBottom;

        // Background
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(dirtyRect);

        DrawGrid(canvas, chartW, chartH);
        DrawAxes(canvas, chartW, chartH, w, h);
        DrawSeries(canvas, chartW, chartH);
    }

    private void DrawGrid(ICanvas canvas, float chartW, float chartH)
    {
        canvas.StrokeColor = GridColor;
        canvas.StrokeSize = 1;

        for (int y = YMin; y <= YMax; y++)
        {
            var yPx = YToPx(y, chartH);
            canvas.DrawLine(MarginLeft, yPx, MarginLeft + chartW, yPx);
        }
    }

    private void DrawAxes(ICanvas canvas, float chartW, float chartH, float w, float h)
    {
        canvas.StrokeColor = AxisColor;
        canvas.StrokeSize = 1.5f;
        canvas.FontSize = 10;
        canvas.FontColor = AxisColor;

        // Y-axis
        canvas.DrawLine(MarginLeft, MarginTop, MarginLeft, MarginTop + chartH);

        // X-axis at y=0
        var zeroY = YToPx(0, chartH);
        canvas.StrokeColor = Color.FromArgb("#AAAAAA");
        canvas.DrawLine(MarginLeft, zeroY, MarginLeft + chartW, zeroY);

        // Y labels
        canvas.StrokeColor = AxisColor;
        for (int y = YMin; y <= YMax; y += 2)
        {
            var yPx = YToPx(y, chartH);
            canvas.DrawString(y > 0 ? $"+{y}" : y.ToString(),
                0, yPx - 6, MarginLeft - 4, 12,
                HorizontalAlignment.Right, VerticalAlignment.Center);
        }

        // X labels: show ~5 evenly spaced dates
        if (Data is null) return;
        int totalDays = (Data.To.ToDateTime(TimeOnly.MinValue) - Data.From.ToDateTime(TimeOnly.MinValue)).Days;
        int labelStep = Math.Max(1, totalDays / 5);

        for (int i = 0; i <= totalDays; i += labelStep)
        {
            var date = Data.From.AddDays(i);
            var xPx = XToPx(date, chartW);
            canvas.DrawString(date.ToString("dd.MM"),
                xPx - 20, MarginTop + chartH + 4, 40, 16,
                HorizontalAlignment.Center, VerticalAlignment.Top);
        }
    }

    private void DrawSeries(ICanvas canvas, float chartW, float chartH)
    {
        if (Data is null || Data.Points.Count == 0) return;

        var sorted = Data.Points.OrderBy(p => p.Date).ToList();

        DrawLine(canvas, sorted, p => p.Mood, MoodColor, chartW, chartH);
        DrawLine(canvas, sorted, p => p.Functionality, FuncColor, chartW, chartH);
        DrawHypomaniaMarkers(canvas, sorted, chartW, chartH);
    }

    private void DrawLine(ICanvas canvas, List<ChartPointDto> points,
        Func<ChartPointDto, int> getValue, Color color, float chartW, float chartH)
    {
        canvas.StrokeColor = color;
        canvas.StrokeSize = 2;
        canvas.FillColor = color;

        PointF? prev = null;
        foreach (var p in points)
        {
            var x = XToPx(p.Date, chartW);
            var y = YToPx(getValue(p), chartH);
            var cur = new PointF(x, y);

            if (prev.HasValue)
                canvas.DrawLine(prev.Value.X, prev.Value.Y, cur.X, cur.Y);

            canvas.FillCircle(x, y, 3);
            prev = cur;
        }
    }

    private void DrawHypomaniaMarkers(ICanvas canvas, List<ChartPointDto> points, float chartW, float chartH)
    {
        foreach (var p in points.Where(p => p.IsHypomanic))
        {
            var x = XToPx(p.Date, chartW);
            // Vertical band across full chart height
            canvas.FillColor = HypomaniaColor.WithAlpha(0.15f);
            canvas.FillRectangle(x - 3, MarginTop, 6, chartH);
            // Diamond marker at top
            canvas.FillColor = HypomaniaColor;
            canvas.FillEllipse(x - 5, MarginTop - 1, 10, 10);
        }
    }

    private float YToPx(int value, float chartH) =>
        MarginTop + (YMax - value) / (float)(YMax - YMin) * chartH;

    private float XToPx(DateOnly date, float chartW)
    {
        if (Data is null) return MarginLeft;
        int totalDays = Math.Max(1,
            (Data.To.ToDateTime(TimeOnly.MinValue) - Data.From.ToDateTime(TimeOnly.MinValue)).Days);
        int dayOffset = (date.ToDateTime(TimeOnly.MinValue) - Data.From.ToDateTime(TimeOnly.MinValue)).Days;
        return MarginLeft + dayOffset / (float)totalDays * chartW;
    }
}
