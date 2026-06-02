using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace LifeChart.Infrastructure.Pdf;

public class PdfSharpRenderer : IPdfRenderer
{
    // A4 in Punkten (1pt = 1/72 Zoll)
    private const double PageW = 595.28;
    private const double PageH = 841.89;
    private const double Margin = 40.0;
    private const double ContentW = PageW - 2 * Margin;
    private const double FooterH = 30.0;
    private const double ContentBottom = PageH - Margin - FooterH;

    // Wong-Palette
    private static readonly XColor MoodColor = XColor.FromArgb(0, 114, 178);
    private static readonly XColor FuncColor = XColor.FromArgb(230, 159, 0);
    private static readonly XColor TextPrimary = XColor.FromArgb(26, 26, 26);
    private static readonly XColor TextSecondary = XColor.FromArgb(107, 107, 107);
    private static readonly XColor GridColor = XColor.FromArgb(200, 200, 200);
    private static readonly XColor BandMania = XColor.FromArgb(30, 255, 230, 0);
    private static readonly XColor BandDepression = XColor.FromArgb(30, 0, 114, 178);

    public Task<byte[]> RenderAsync(PdfReportData data)
    {
        var document = new PdfDocument();
        document.Info.Title = "LifeChart Report";
        document.Info.Author = "LifeChart";

        var ctx = new PageContext(document);

        DrawHeader(ctx, data);
        DrawChart(ctx, data.ChartData);
        DrawLegend(ctx);
        DrawDailyTable(ctx, data.Entries);
        DrawNotes(ctx, data.Entries);
        DrawMedications(ctx, data.ActiveMedications);
        DrawFooter(ctx);

        using var stream = new MemoryStream();
        document.Save(stream);
        return Task.FromResult(stream.ToArray());
    }

    // ── Header ──────────────────────────────────────────────────────────────

    private void DrawHeader(PageContext ctx, PdfReportData data)
    {
        var gfx = ctx.Gfx;

        var titleFont = new XFont("Arial", 14, XFontStyle.Bold);
        var subFont = new XFont("Arial", 9, XFontStyle.Regular);

        gfx.DrawString("LifeChart — Mood & Functionality Report",
            titleFont, new XSolidBrush(TextPrimary),
            new XRect(Margin, ctx.Y, ContentW, 20), XStringFormats.TopLeft);

        ctx.Y += 22;

        var periodLabel = $"{data.From:dd.MM.yyyy} – {data.To:dd.MM.yyyy}";
        gfx.DrawString(periodLabel, subFont,
            new XSolidBrush(TextSecondary),
            new XRect(Margin, ctx.Y, ContentW, 14), XStringFormats.TopLeft);

        ctx.Y += 18;

        // Trennlinie
        gfx.DrawLine(new XPen(GridColor, 0.5), Margin, ctx.Y, PageW - Margin, ctx.Y);
        ctx.Y += 10;
    }

    // ── Chart ───────────────────────────────────────────────────────────────

    private const double ChartH = 220.0;

    private void DrawChart(PageContext ctx, ChartDataDto chartData)
    {
        ctx.EnsureSpace(ChartH + 20, this);
        var gfx = ctx.Gfx;

        double chartX = Margin + 30;
        double chartY = ctx.Y;
        double chartW = ContentW - 30;

        // Hintergrundbänder
        gfx.DrawRectangle(new XSolidBrush(BandMania),
            chartX, chartY, chartW, ChartH / 2);
        gfx.DrawRectangle(new XSolidBrush(BandDepression),
            chartX, chartY + ChartH / 2, chartW, ChartH / 2);

        // Gitternetzlinien + Y-Achsenbeschriftung
        var gridFont = new XFont("Arial", 7, XFontStyle.Regular);
        for (int v = -5; v <= 5; v++)
        {
            double y = ValueToY(v, chartY, ChartH);
            var pen = v == 0
                ? new XPen(XColor.FromArgb(80, 0, 0, 0), 0.7)
                : new XPen(XColor.FromArgb(40, 0, 0, 0), 0.4);

            gfx.DrawLine(pen, chartX, y, chartX + chartW, y);

            gfx.DrawString(v.ToString(), gridFont,
                new XSolidBrush(TextSecondary),
                new XRect(Margin, y - 5, 25, 10), XStringFormats.TopRight);
        }

        // Rahmen
        gfx.DrawRectangle(new XPen(GridColor, 0.5), chartX, chartY, chartW, ChartH);

        if (chartData.Points.Count == 0)
        {
            var noDataFont = new XFont("Arial", 10, XFontStyle.Regular);
            gfx.DrawString("Keine Daten für diesen Zeitraum.",
                noDataFont, new XSolidBrush(TextSecondary),
                new XRect(chartX, chartY, chartW, ChartH), XStringFormats.Center);
            ctx.Y += ChartH + 8;
            return;
        }

        int n = chartData.Points.Count;

        // Datenserien zeichnen
        DrawSeries(gfx, chartData.Points, n, chartX, chartY, chartW,
            p => p.Mood, MoodColor, dashStyle: false);
        DrawSeries(gfx, chartData.Points, n, chartX, chartY, chartW,
            p => p.Functionality, FuncColor, dashStyle: true);

        // X-Achse: Datumsmarkierungen
        DrawXDates(gfx, chartData.Points, n, chartX, chartY, chartW);

        ctx.Y += ChartH + 8;
    }

    private void DrawSeries(
        XGraphics gfx,
        IReadOnlyList<ChartPointDto> points,
        int n,
        double chartX, double chartY, double chartW,
        Func<ChartPointDto, int> valueSelector,
        XColor color,
        bool dashStyle)
    {
        var pen = new XPen(color, 1.5);
        if (dashStyle) pen.DashStyle = XDashStyle.Dash;
        var brush = new XSolidBrush(color);

        // Linie
        for (int i = 1; i < n; i++)
        {
            double x1 = PointToX(i - 1, n, chartX, chartW);
            double y1 = ValueToY(valueSelector(points[i - 1]), chartY, ChartH);
            double x2 = PointToX(i, n, chartX, chartW);
            double y2 = ValueToY(valueSelector(points[i]), chartY, ChartH);
            gfx.DrawLine(pen, x1, y1, x2, y2);
        }

        // Datenpunkte
        for (int i = 0; i < n; i++)
        {
            double x = PointToX(i, n, chartX, chartW);
            double y = ValueToY(valueSelector(points[i]), chartY, ChartH);
            const double r = 2.5;

            if (dashStyle)
                gfx.DrawRectangle(brush, x - r, y - r, r * 2, r * 2);
            else
                gfx.DrawEllipse(brush, x - r, y - r, r * 2, r * 2);
        }
    }

    private void DrawXDates(
        XGraphics gfx,
        IReadOnlyList<ChartPointDto> points,
        int n,
        double chartX, double chartY, double chartW)
    {
        var font = new XFont("Arial", 6.5, XFontStyle.Regular);
        int step = Math.Max(1, n / 9);

        for (int i = 0; i < n; i++)
        {
            if (i % step != 0 && i != n - 1) continue;
            double x = PointToX(i, n, chartX, chartW);
            string label = points[i].Date.ToString("dd.MM");
            gfx.DrawString(label, font, new XSolidBrush(TextSecondary),
                new XRect(x - 15, chartY + ChartH + 2, 30, 10),
                XStringFormats.TopCenter);
        }
    }

    // ── Legende ─────────────────────────────────────────────────────────────

    private void DrawLegend(PageContext ctx)
    {
        var gfx = ctx.Gfx;
        var font = new XFont("Arial", 8, XFontStyle.Regular);
        double y = ctx.Y;

        // Stimmung
        gfx.DrawRectangle(new XSolidBrush(MoodColor), Margin + 30, y, 20, 4);
        gfx.DrawString("Stimmung", font, new XSolidBrush(TextPrimary),
            new XRect(Margin + 56, y - 2, 80, 12), XStringFormats.TopLeft);

        // Funktionsfähigkeit (gestrichelt)
        var dashedPen = new XPen(FuncColor, 1.5) { DashStyle = XDashStyle.Dash };
        gfx.DrawLine(dashedPen, Margin + 150, y + 2, Margin + 170, y + 2);
        gfx.DrawString("Funktionsfähigkeit", font, new XSolidBrush(TextPrimary),
            new XRect(Margin + 176, y - 2, 120, 12), XStringFormats.TopLeft);

        ctx.Y += 20;

        // Skalenbeschreibung
        var scaleFont = new XFont("Arial", 7, XFontStyle.Regular);
        gfx.DrawString("+5 = Manie  ·  0 = Normal  ·  −5 = Schwere Depression",
            scaleFont, new XSolidBrush(TextSecondary),
            new XRect(Margin + 30, ctx.Y, ContentW, 10), XStringFormats.TopLeft);

        ctx.Y += 16;

        // Trennlinie
        gfx.DrawLine(new XPen(GridColor, 0.5), Margin, ctx.Y, PageW - Margin, ctx.Y);
        ctx.Y += 12;
    }

    // ── Tagesübersicht ──────────────────────────────────────────────────────

    private void DrawDailyTable(PageContext ctx, IReadOnlyList<DailyEntryDto> entries)
    {
        if (entries.Count == 0) return;

        ctx.EnsureSpace(60, this);
        var gfx = ctx.Gfx;

        var headerFont = new XFont("Arial", 9, XFontStyle.Bold);
        var cellFont = new XFont("Arial", 8, XFontStyle.Regular);

        // Tabellenüberschrift
        gfx.DrawString("Tagesübersicht", headerFont,
            new XSolidBrush(TextPrimary),
            new XRect(Margin, ctx.Y, ContentW, 14), XStringFormats.TopLeft);
        ctx.Y += 16;

        // Spaltenbreiten
        double[] colW = [70, 45, 50, 45, ContentW - 210];
        string[] headers = ["Datum", "Schlaf", "Medik.", "Mens.", "Symptome"];

        // Header-Zeile
        DrawTableRow(gfx, ctx.Y, colW, headers, headerFont, isHeader: true);
        ctx.Y += 16;

        foreach (var entry in entries)
        {
            ctx.EnsureSpace(14, this);

            string symptoms = entry.Symptoms is { Length: > 0 }
                ? entry.Symptoms.Length > 40
                    ? entry.Symptoms[..37] + "..."
                    : entry.Symptoms
                : "-";

            string[] cells =
            [
                entry.Date.ToString("dd.MM.yyyy"),
                $"{entry.SleepHours}h",
                entry.MedicationTaken ? "✓" : "-",
                entry.MenstrualCycle ? "✓" : "-",
                symptoms
            ];

            DrawTableRow(gfx, ctx.Y, colW, cells, cellFont, isHeader: false);
            ctx.Y += 14;
        }

        ctx.Y += 10;

        gfx.DrawLine(new XPen(GridColor, 0.5), Margin, ctx.Y, PageW - Margin, ctx.Y);
        ctx.Y += 12;
    }

    private void DrawTableRow(
        XGraphics gfx, double y, double[] colW, string[] cells,
        XFont font, bool isHeader)
    {
        var brush = new XSolidBrush(isHeader ? TextPrimary : TextSecondary);
        if (isHeader)
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(240, 240, 240)),
                Margin, y, ContentW, 14);

        double x = Margin;
        for (int i = 0; i < cells.Length; i++)
        {
            gfx.DrawString(cells[i], font, brush,
                new XRect(x + 2, y + 2, colW[i] - 4, 12), XStringFormats.TopLeft);
            x += colW[i];
        }

        gfx.DrawLine(new XPen(GridColor, 0.3), Margin, y + 14, PageW - Margin, y + 14);
    }

    // ── Notizen ─────────────────────────────────────────────────────────────

    private void DrawNotes(PageContext ctx, IReadOnlyList<DailyEntryDto> entries)
    {
        var withNotes = entries.Where(e => !string.IsNullOrWhiteSpace(e.Notes)).ToList();
        if (withNotes.Count == 0) return;

        ctx.EnsureSpace(30, this);
        var gfx = ctx.Gfx;

        var headerFont = new XFont("Arial", 9, XFontStyle.Bold);
        var noteFont = new XFont("Arial", 8, XFontStyle.Regular);
        var dateFont = new XFont("Arial", 8, XFontStyle.Bold);

        gfx.DrawString("Notizen", headerFont,
            new XSolidBrush(TextPrimary),
            new XRect(Margin, ctx.Y, ContentW, 14), XStringFormats.TopLeft);
        ctx.Y += 16;

        foreach (var entry in withNotes)
        {
            ctx.EnsureSpace(24, this);

            gfx.DrawString(entry.Date.ToString("dd.MM.yyyy") + ":", dateFont,
                new XSolidBrush(TextPrimary),
                new XRect(Margin, ctx.Y, 70, 12), XStringFormats.TopLeft);

            gfx.DrawString(entry.Notes!, noteFont,
                new XSolidBrush(TextSecondary),
                new XRect(Margin + 75, ctx.Y, ContentW - 75, 24),
                XStringFormats.TopLeft);

            ctx.Y += 20;
        }

        ctx.Y += 8;
        gfx.DrawLine(new XPen(GridColor, 0.5), Margin, ctx.Y, PageW - Margin, ctx.Y);
        ctx.Y += 12;
    }

    // ── Medikamente ─────────────────────────────────────────────────────────

    private void DrawMedications(PageContext ctx, IReadOnlyList<MedicationDto> medications)
    {
        if (medications.Count == 0) return;

        ctx.EnsureSpace(30, this);
        var gfx = ctx.Gfx;

        var headerFont = new XFont("Arial", 9, XFontStyle.Bold);
        var itemFont = new XFont("Arial", 8, XFontStyle.Regular);

        gfx.DrawString("Aktuelle Medikamente", headerFont,
            new XSolidBrush(TextPrimary),
            new XRect(Margin, ctx.Y, ContentW, 14), XStringFormats.TopLeft);
        ctx.Y += 16;

        foreach (var med in medications)
        {
            ctx.EnsureSpace(14, this);

            var times = string.Join(", ",
                med.IntakeTimes.Select(i => $"{i.Time} ({i.DoseCount}x)"));

            string line = $"• {med.Name}  {med.Dosage}";
            if (!string.IsNullOrEmpty(times))
                line += $"  —  {times}";

            gfx.DrawString(line, itemFont, new XSolidBrush(TextPrimary),
                new XRect(Margin, ctx.Y, ContentW, 12), XStringFormats.TopLeft);

            ctx.Y += 14;
        }
    }

    // ── Footer ──────────────────────────────────────────────────────────────

    private void DrawFooter(PageContext ctx)
    {
        // Footer auf jede Seite zeichnen
        foreach (var (page, gfx) in ctx.AllPages)
        {
            double lineY = PageH - Margin - 16;

            gfx.DrawLine(new XPen(GridColor, 0.4), Margin, lineY, PageW - Margin, lineY);

            var font = new XFont("Arial", 7, XFontStyle.Regular);
            var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            gfx.DrawString($"Erstellt von LifeChart am {ts}",
                font, new XSolidBrush(TextSecondary),
                new XRect(Margin, lineY + 3, ContentW / 2, 10), XStringFormats.TopLeft);

            gfx.DrawString("Basierend auf der NIMH Life Chart Methode",
                font, new XSolidBrush(TextSecondary),
                new XRect(Margin + ContentW / 2, lineY + 3, ContentW / 2, 10),
                XStringFormats.TopRight);
        }
    }

    // ── Hilfsklassen ────────────────────────────────────────────────────────

    private static double ValueToY(int value, double chartY, double chartH)
        => chartY + chartH - ((value + 5.0) / 10.0) * chartH;

    private static double PointToX(int index, int n, double chartX, double chartW)
        => chartX + ((double)index / Math.Max(n - 1, 1)) * chartW;

    private sealed class PageContext
    {
        private readonly PdfDocument _document;
        private readonly List<(PdfPage page, XGraphics gfx)> _pages = [];

        public XGraphics Gfx => _pages[^1].gfx;
        public double Y { get; set; }

        public IEnumerable<(PdfPage, XGraphics)> AllPages => _pages;

        public PageContext(PdfDocument document)
        {
            _document = document;
            AddPage();
            Y = Margin;
        }

        public void EnsureSpace(double needed, PdfSharpRenderer renderer)
        {
            if (Y + needed > ContentBottom)
            {
                AddPage();
                Y = Margin;
            }
        }

        private void AddPage()
        {
            var page = _document.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            _pages.Add((page, XGraphics.FromPdfPage(page)));
        }
    }
}
