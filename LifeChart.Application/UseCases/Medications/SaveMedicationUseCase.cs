using LifeChart.Application.DTOs;
using LifeChart.Domain.Medications;

namespace LifeChart.Application.UseCases.Medications;

public class SaveMedicationUseCase
{
    private readonly IMedicationRepository _repository;

    public SaveMedicationUseCase(IMedicationRepository repository)
        => _repository = repository;

    public async Task ExecuteAsync(SaveMedicationDto dto)
    {
        var intakeTimes = dto.IntakeTimes
            .Select(i => new IntakeTime(TimeOnly.Parse(i.Time), i.DoseCount))
            .ToList();

        if (dto.Id == 0)
        {
            var medication = new Medication(
                dto.Name,
                dto.Dosage,
                dto.MinStock,
                dto.CurrentStock,
                intakeTimes);

            await _repository.SaveAsync(medication);
        }
        else
        {
            var existing = await _repository.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException(
                    $"Medikament mit Id {dto.Id} nicht gefunden.");

            existing.Update(
                dto.Name,
                dto.Dosage,
                dto.MinStock,
                dto.CurrentStock,
                intakeTimes);

            await _repository.SaveAsync(existing);
        }
    }
}
