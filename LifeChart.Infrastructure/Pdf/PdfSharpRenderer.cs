using LifeChart.Application.Interfaces;

namespace LifeChart.Infrastructure.Pdf;

public class PdfSharpRenderer : IPdfRenderer
{
    public Task<byte[]> RenderAsync(PdfReportData data) => throw new NotImplementedException();
}
