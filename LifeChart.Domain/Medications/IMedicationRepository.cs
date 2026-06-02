namespace LifeChart.Domain.Medications;

public interface IMedicationRepository
{
    Task<IEnumerable<Medication>> GetAllActiveAsync();
    Task<Medication?> GetByIdAsync(int id);
    Task SaveAsync(Medication medication);
    Task DeleteAsync(int id);
}
