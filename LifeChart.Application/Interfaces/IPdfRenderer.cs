using LifeChart.Application.DTOs;

namespace LifeChart.Application.Interfaces;

public record PdfReportData(
    ChartDataDto ChartData,
    IReadOnlyList<DailyEntryDto> Entries,
    IReadOnlyList<MedicationDto> ActiveMedications,
    DateOnly From,
    DateOnly To
);

public interface IPdfRenderer
{
    Task<byte[]> RenderAsync(PdfReportData data);
}
