using LifeChart.Application.DTOs;
using Microsoft.Maui.Graphics;

namespace LifeChart.Linux.Pages;

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

        canvas.DrawLine(MarginLeft, MarginTop, MarginLeft, MarginTop + chartH);

        var zeroY = YToPx(0, chartH);
        canvas.StrokeColor = Color.FromArgb("#AAAAAA");
        canvas.DrawLine(MarginLeft, zeroY, MarginLeft + chartW, zeroY);

        canvas.StrokeColor = AxisColor;
        for (int y = YMin; y <= YMax; y += 2)
        {
            var yPx = YToPx(y, chartH);
            canvas.DrawString(y > 0 ? $"+{y}" : y.ToString(),
                0, yPx - 6, MarginLeft - 4, 12,
                HorizontalAlignment.Right, VerticalAlignment.Center);
        }

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
            canvas.FillColor = HypomaniaColor.WithAlpha(0.15f);
            canvas.FillRectangle(x - 3, MarginTop, 6, chartH);
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
