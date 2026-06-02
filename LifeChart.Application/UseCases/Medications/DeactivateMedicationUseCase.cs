using LifeChart.Domain.Medications;

namespace LifeChart.Application.UseCases.Medications;

public class DeactivateMedicationUseCase
{
    private readonly IMedicationRepository _repository;

    public DeactivateMedicationUseCase(IMedicationRepository repository)
        => _repository = repository;

    public async Task ExecuteAsync(int id)
        => await _repository.DeleteAsync(id);
}
