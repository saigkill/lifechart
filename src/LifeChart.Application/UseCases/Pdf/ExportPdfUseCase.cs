using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;
using LifeChart.Domain.Entries;
using LifeChart.Domain.Medications;

namespace LifeChart.Application.UseCases.Pdf;

public class ExportPdfUseCase
{
    private readonly IDailyEntryRepository _entryRepository;
    private readonly IMedicationRepository _medicationRepository;
    private readonly IPdfRenderer _renderer;

    public ExportPdfUseCase(
        IDailyEntryRepository entryRepository,
        IMedicationRepository medicationRepository,
        IPdfRenderer renderer)
    {
        _entryRepository = entryRepository;
        _medicationRepository = medicationRepository;
        _renderer = renderer;
    }

    public async Task<byte[]> ExecuteAsync(DateOnly from, DateOnly to)
    {
        var entries = (await _entryRepository.GetRangeAsync(from, to))
            .Select(e => new DailyEntryDto(
                e.Date, e.Mood.Value, e.Functionality.Value,
                e.SleepHours.Value, e.MedicationTaken,
                e.MenstrualCycle, e.Symptoms, e.Notes, e.IsHypomanic))
            .ToList()
            .AsReadOnly();

        var medications = (await _medicationRepository.GetAllActiveAsync())
            .Select(m => new MedicationDto(
                m.Id, m.Name, m.Dosage,
                m.MinStock, m.CurrentStock, m.IsStockLow,
                m.IntakeTimes
                    .Select(i => new IntakeTimeDto(i.Time.ToString("HH:mm"), i.DoseCount))
                    .ToList()
                    .AsReadOnly()))
            .ToList()
            .AsReadOnly();

        var chartData = new ChartDataDto(
            entries
                .Select(e => new ChartPointDto(e.Date, e.Mood, e.Functionality, e.IsHypomanic))
                .ToList()
                .AsReadOnly(),
            from, to);

        return await _renderer.RenderAsync(
            new PdfReportData(chartData, entries, medications, from, to));
    }
}
