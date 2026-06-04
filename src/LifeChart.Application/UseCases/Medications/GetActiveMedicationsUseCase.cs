using LifeChart.Application.DTOs;
using LifeChart.Domain.Medications;

namespace LifeChart.Application.UseCases.Medications;

public class GetActiveMedicationsUseCase
{
    private readonly IMedicationRepository _repository;

    public GetActiveMedicationsUseCase(IMedicationRepository repository)
        => _repository = repository;

    public async Task<IEnumerable<MedicationDto>> ExecuteAsync()
    {
        var medications = await _repository.GetAllActiveAsync();
        return medications.Select(m => new MedicationDto(
            m.Id, m.Name, m.Dosage,
            m.MinStock, m.CurrentStock, m.IsStockLow,
            m.IntakeTimes
                .Select(i => new IntakeTimeDto(i.Time.ToString("HH:mm"), i.DoseCount))
                .ToList()
                .AsReadOnly()));
    }
}
