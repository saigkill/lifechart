using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;
using SkiaSharp;

namespace LifeChart.Infrastructure.Pdf;

public class PdfRenderer : IPdfRenderer
{
    private const float PageW = 595.28f;
    private const float PageH = 841.89f;
    private const float Margin = 40f;
    private const float ContentW = PageW - 2 * Margin;
    private const float FooterH = 30f;
    private const float ContentBottom = PageH - Margin - FooterH;
    private const float ChartH = 220f;

    private static readonly SKColor MoodColor = new(0, 114, 178);
    private static readonly SKColor FuncColor = new(230, 159, 0);
    private static readonly SKColor TextPrimary = new(26, 26, 26);
    private static readonly SKColor TextSecondary = new(107, 107, 107);
    private static readonly SKColor GridColor = new(200, 200, 200);
    private static readonly SKColor BandMania = new(255, 230, 0, 30);
    private static readonly SKColor BandDepression = new(0, 114, 178, 30);

    private static readonly SKTypeface Arial =
        SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

    private static readonly SKTypeface ArialBold =
        SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

    public Task<byte[]> RenderAsync(PdfReportData data)
    {
        using var stream = new MemoryStream();
        using var wstream = new SKManagedWStream(stream);
        using var document = SKDocument.CreatePdf(wstream);

        var ctx = new PageContext(document);
        DrawHeader(ctx, data);
        DrawChart(ctx, data.ChartData);
        DrawLegend(ctx);
        DrawDailyTable(ctx, data.Entries);
        DrawNotes(ctx, data.Entries);
        DrawMedications(ctx, data.ActiveMedications);
        DrawFooter(ctx);
        ctx.Close();

        return Task.FromResult(stream.ToArray());
    }

    private static SKPaint Fill(SKColor color)
        => new() { Color = color, Style = SKPaintStyle.Fill, IsAntialias = true };

    private static SKPaint Stroke(SKColor color, float strokeWidth)
        => new() { Color = color, StrokeWidth = strokeWidth, Style = SKPaintStyle.Stroke, IsAntialias = true };

    private static void DrawText(SKCanvas canvas, string text, SKRect rect, SKFont font, SKPaint paint,
        bool centerX = false, bool centerY = false)
    {
        font.GetFontMetrics(out var metrics);
        var textWidth = font.MeasureText(text);
        var textHeight = metrics.Descent - metrics.Ascent;

        var x = centerX ? rect.MidX - textWidth / 2 : rect.Left;
        var y = centerY ? rect.MidY - textHeight / 2 : rect.Top;

        canvas.DrawText(text, x, y - metrics.Ascent, font, paint);
    }

    // ── Header ──────────────────────────────────────────────────────────────

    private void DrawHeader(PageContext ctx, PdfReportData data)
    {
        var canvas = ctx.Canvas;

        using var titleFont = new SKFont(ArialBold, 14);
        using var titlePaint = Fill(TextPrimary);
        DrawText(canvas, "LifeChart — Mood & Functionality Report",
            new SKRect(Margin, ctx.Y, Margin + ContentW, ctx.Y + 20), titleFont, titlePaint);
        ctx.Y += 22;

        var periodLabel = $"{data.From:dd.MM.yyyy} – {data.To:dd.MM.yyyy}";
        using var subFont = new SKFont(Arial, 9);
        using var subPaint = Fill(TextSecondary);
        DrawText(canvas, periodLabel,
            new SKRect(Margin, ctx.Y, Margin + ContentW, ctx.Y + 14), subFont, subPaint);
        ctx.Y += 18;

        using var linePaint = Stroke(GridColor, 0.5f);
        canvas.DrawLine(Margin, ctx.Y, PageW - Margin, ctx.Y, linePaint);
        ctx.Y += 10;
    }

    // ── Chart ───────────────────────────────────────────────────────────────

    private void DrawChart(PageContext ctx, ChartDataDto chartData)
    {
        ctx.EnsureSpace(ChartH + 20);
        var canvas = ctx.Canvas;

        var chartX = Margin + 30;
        var chartY = ctx.Y;
        var chartW = ContentW - 30;

        using var maniaFill = Fill(BandMania);
        canvas.DrawRect(chartX, chartY, chartW, ChartH / 2, maniaFill);

        using var depFill = Fill(BandDepression);
        canvas.DrawRect(chartX, chartY + ChartH / 2, chartW, ChartH / 2, depFill);

        using var gridFont = new SKFont(Arial, 7);
        using var gridPaint = Fill(TextSecondary);
        for (var v = -5; v <= 5; v++)
        {
            var y = ValueToY(v, chartY);
            var penColor = v == 0
                ? new SKColor(0, 0, 0, 80)
                : new SKColor(0, 0, 0, 40);
            using var pen = Stroke(penColor, v == 0 ? 0.7f : 0.4f);
            canvas.DrawLine(chartX, y, chartX + chartW, y, pen);

            DrawText(canvas, v.ToString(),
                new SKRect(Margin, y - 5, Margin + 25, y + 5), gridFont, gridPaint);
        }

        using var borderPen = Stroke(GridColor, 0.5f);
        canvas.DrawRect(chartX, chartY, chartW, ChartH, borderPen);

        if (chartData.Points.Count == 0)
        {
            using var noDataFont = new SKFont(Arial, 10);
            DrawText(canvas, "Keine Daten für diesen Zeitraum.",
                new SKRect(chartX, chartY, chartX + chartW, chartY + ChartH), noDataFont, gridPaint,
                centerX: true, centerY: true);
            ctx.Y += ChartH + 8;
            return;
        }

        var n = chartData.Points.Count;

        DrawSeries(canvas, chartData.Points, n, chartX, chartY, chartW,
            p => p.Mood, MoodColor, dashStyle: false);
        DrawSeries(canvas, chartData.Points, n, chartX, chartY, chartW,
            p => p.Functionality, FuncColor, dashStyle: true);

        DrawXDates(canvas, chartData.Points, n, chartX, chartY, chartW, gridFont, gridPaint);

        ctx.Y += ChartH + 8;
    }

    private static void DrawSeries(
        SKCanvas canvas,
        IReadOnlyList<ChartPointDto> points,
        int n,
        float chartX, float chartY, float chartW,
        Func<ChartPointDto, int> valueSelector,
        SKColor color,
        bool dashStyle)
    {
        using var pen = Stroke(color, 1.5f);
        if (dashStyle)
            pen.PathEffect = SKPathEffect.CreateDash([5, 5], 0);

        for (var i = 1; i < n; i++)
        {
            var x1 = PointToX(i - 1, n, chartX, chartW);
            var y1 = ValueToY(valueSelector(points[i - 1]), chartY);
            var x2 = PointToX(i, n, chartX, chartW);
            var y2 = ValueToY(valueSelector(points[i]), chartY);
            canvas.DrawLine(x1, y1, x2, y2, pen);
        }

        using var brush = Fill(color);
        const float r = 2.5f;
        for (var i = 0; i < n; i++)
        {
            var x = PointToX(i, n, chartX, chartW);
            var y = ValueToY(valueSelector(points[i]), chartY);

            if (dashStyle)
                canvas.DrawRect(x - r, y - r, r * 2, r * 2, brush);
            else
                canvas.DrawCircle(x, y, r, brush);
        }
    }

    private static void DrawXDates(
        SKCanvas canvas,
        IReadOnlyList<ChartPointDto> points,
        int n,
        float chartX, float chartY, float chartW,
        SKFont font, SKPaint paint)
    {
        var step = Math.Max(1, n / 9);

        for (var i = 0; i < n; i++)
        {
            if (i % step != 0 && i != n - 1) continue;
            var x = PointToX(i, n, chartX, chartW);
            var label = points[i].Date.ToString("dd.MM");
            DrawText(canvas, label,
                new SKRect(x - 15, chartY + ChartH + 2, x + 15, chartY + ChartH + 12), font, paint,
                centerX: true);
        }
    }

    // ── Legende ─────────────────────────────────────────────────────────────

    private void DrawLegend(PageContext ctx)
    {
        var canvas = ctx.Canvas;
        var y = ctx.Y;

        using var moodFill = Fill(MoodColor);
        canvas.DrawRect(Margin + 30, y, 20, 4, moodFill);

        using var legendFont = new SKFont(Arial, 8);
        using var legendPaint = Fill(TextPrimary);
        DrawText(canvas, "Stimmung",
            new SKRect(Margin + 56, y - 2, Margin + 136, y + 10), legendFont, legendPaint);

        using var dashedPen = Stroke(FuncColor, 1.5f);
        dashedPen.PathEffect = SKPathEffect.CreateDash([5, 5], 0);
        canvas.DrawLine(Margin + 150, y + 2, Margin + 170, y + 2, dashedPen);

        DrawText(canvas, "Funktionsfähigkeit",
            new SKRect(Margin + 176, y - 2, Margin + 296, y + 10), legendFont, legendPaint);

        ctx.Y += 20;

        using var scaleFont = new SKFont(Arial, 7);
        DrawText(canvas, "+5 = Manie  ·  0 = Normal  ·  −5 = Schwere Depression",
            new SKRect(Margin + 30, ctx.Y, Margin + 30 + ContentW, ctx.Y + 10), scaleFont, legendPaint);

        ctx.Y += 16;

        using var linePaint = Stroke(GridColor, 0.5f);
        canvas.DrawLine(Margin, ctx.Y, PageW - Margin, ctx.Y, linePaint);
        ctx.Y += 12;
    }

    // ── Tagesübersicht ──────────────────────────────────────────────────────

    private void DrawDailyTable(PageContext ctx, IReadOnlyList<DailyEntryDto> entries)
    {
        if (entries.Count == 0) return;

        ctx.EnsureSpace(60);
        var canvas = ctx.Canvas;

        using var headerFont = new SKFont(ArialBold, 9);
        using var headerPaint = Fill(TextPrimary);
        DrawText(canvas, "Tagesübersicht",
            new SKRect(Margin, ctx.Y, Margin + ContentW, ctx.Y + 14), headerFont, headerPaint);
        ctx.Y += 16;

        float[] colW = [70, 45, 50, 45, ContentW - 210];
        string[] headers = ["Datum", "Schlaf", "Medik.", "Mens.", "Symptome"];

        DrawTableRow(canvas, ctx.Y, colW, headers, headerFont, headerPaint, isHeader: true);
        ctx.Y += 16;

        using var cellFont = new SKFont(Arial, 8);
        using var cellPaint = Fill(TextSecondary);
        foreach (var entry in entries)
        {
            ctx.EnsureSpace(14);

            var symptoms = entry.Symptoms is { Length: > 0 }
                ? entry.Symptoms
                : "-";

            if (symptoms.Length > 40)
                symptoms = symptoms[..37] + "...";

            string[] cells =
            [
                entry.Date.ToString("dd.MM.yyyy"),
                $"{entry.SleepHours}h",
                entry.MedicationTaken ? "✓" : "-",
                entry.MenstrualCycle ? "✓" : "-",
                symptoms
            ];

            DrawTableRow(canvas, ctx.Y, colW, cells, cellFont, cellPaint, isHeader: false);
            ctx.Y += 14;
        }

        ctx.Y += 10;

        using var linePaint = Stroke(GridColor, 0.5f);
        canvas.DrawLine(Margin, ctx.Y, PageW - Margin, ctx.Y, linePaint);
        ctx.Y += 12;
    }

    private static void DrawTableRow(
        SKCanvas canvas, float y, float[] colW, string[] cells,
        SKFont font, SKPaint paint, bool isHeader)
    {
        if (isHeader)
        {
            using var bg = Fill(new SKColor(240, 240, 240));
            canvas.DrawRect(Margin, y, ContentW, 14, bg);
        }

        var x = Margin;
        for (var i = 0; i < cells.Length; i++)
        {
            DrawText(canvas, cells[i],
                new SKRect(x + 2, y + 2, x + colW[i] - 2, y + 14), font, paint);
            x += colW[i];
        }

        using var linePaint = Stroke(GridColor, 0.3f);
        canvas.DrawLine(Margin, y + 14, PageW - Margin, y + 14, linePaint);
    }

    // ── Notizen ─────────────────────────────────────────────────────────────

    private void DrawNotes(PageContext ctx, IReadOnlyList<DailyEntryDto> entries)
    {
        var withNotes = entries.Where(e => !string.IsNullOrWhiteSpace(e.Notes)).ToList();
        if (withNotes.Count == 0) return;

        ctx.EnsureSpace(30);
        var canvas = ctx.Canvas;

        using var headerFont = new SKFont(ArialBold, 9);
        using var headerPaint = Fill(TextPrimary);
        DrawText(canvas, "Notizen",
            new SKRect(Margin, ctx.Y, Margin + ContentW, ctx.Y + 14), headerFont, headerPaint);
        ctx.Y += 16;

        using var noteFont = new SKFont(Arial, 8);
        using var notePaint = Fill(TextSecondary);
        foreach (var entry in withNotes)
        {
            ctx.EnsureSpace(24);

            using var dateFont = new SKFont(ArialBold, 8);
            DrawText(canvas, entry.Date.ToString("dd.MM.yyyy") + ":",
                new SKRect(Margin, ctx.Y, Margin + 70, ctx.Y + 12), dateFont, headerPaint);

            DrawText(canvas, entry.Notes!,
                new SKRect(Margin + 75, ctx.Y, Margin + 75 + ContentW - 75, ctx.Y + 24), noteFont, notePaint);

            ctx.Y += 20;
        }

        ctx.Y += 8;
        using var linePaint = Stroke(GridColor, 0.5f);
        canvas.DrawLine(Margin, ctx.Y, PageW - Margin, ctx.Y, linePaint);
        ctx.Y += 12;
    }

    // ── Medikamente ─────────────────────────────────────────────────────────

    private void DrawMedications(PageContext ctx, IReadOnlyList<MedicationDto> medications)
    {
        if (medications.Count == 0) return;

        ctx.EnsureSpace(30);
        var canvas = ctx.Canvas;

        using var headerFont = new SKFont(ArialBold, 9);
        using var headerPaint = Fill(TextPrimary);
        DrawText(canvas, "Aktuelle Medikamente",
            new SKRect(Margin, ctx.Y, Margin + ContentW, ctx.Y + 14), headerFont, headerPaint);
        ctx.Y += 16;

        using var itemFont = new SKFont(Arial, 8);
        foreach (var med in medications)
        {
            ctx.EnsureSpace(14);

            var times = string.Join(", ",
                med.IntakeTimes.Select(i => $"{i.Time} ({i.DoseCount}x)"));

            var line = $"• {med.Name}  {med.Dosage}";
            if (!string.IsNullOrEmpty(times))
                line += $"  —  {times}";

            DrawText(canvas, line,
                new SKRect(Margin, ctx.Y, Margin + ContentW, ctx.Y + 12), itemFont, headerPaint);

            ctx.Y += 14;
        }
    }

    // ── Footer ──────────────────────────────────────────────────────────────

    private void DrawFooter(PageContext ctx)
    {
        foreach (var canvas in ctx.AllPages)
        {
            var lineY = PageH - Margin - 16;

            using var linePaint = Stroke(GridColor, 0.4f);
            canvas.DrawLine(Margin, lineY, PageW - Margin, lineY, linePaint);

            using var font = new SKFont(Arial, 7);
            using var paint = Fill(TextSecondary);
            var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            DrawText(canvas, $"Erstellt von LifeChart am {ts}",
                new SKRect(Margin, lineY + 3, Margin + ContentW / 2, lineY + 13), font, paint);
            DrawText(canvas, "Basierend auf der NIMH Life Chart Methode",
                new SKRect(Margin + ContentW / 2, lineY + 3, PageW - Margin, lineY + 13), font, paint,
                centerX: true);
        }
    }

    // ── Hilfsfunktionen ─────────────────────────────────────────────────────

    private static float ValueToY(int value, float chartY)
        => chartY + ChartH - ((value + 5f) / 10f) * ChartH;

    private static float PointToX(int index, int n, float chartX, float chartW)
        => chartX + ((float)index / Math.Max(n - 1, 1)) * chartW;

    // ── PageContext ─────────────────────────────────────────────────────────

    private sealed class PageContext
    {
        private readonly SKDocument _document;
        private readonly List<SKCanvas> _pages = [];
        private bool _pageOpen;

        public SKCanvas Canvas => _pages[^1];
        public float Y { get; set; }
        public IReadOnlyList<SKCanvas> AllPages => _pages;

        public PageContext(SKDocument document)
        {
            _document = document;
            AddPage();
            Y = Margin;
        }

        public void EnsureSpace(float needed)
        {
            if (Y + needed > ContentBottom)
            {
                AddPage();
                Y = Margin;
            }
        }

        private void AddPage()
        {
            if (_pageOpen)
                _document.EndPage();
            _pages.Add(_document.BeginPage(PageW, PageH));
            _pageOpen = true;
        }

        public void Close()
        {
            if (_pageOpen)
            {
                _document.EndPage();
                _pageOpen = false;
            }
            _document.Close();
        }
    }
}
