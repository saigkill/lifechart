using LifeChart.Domain.Medications;
using LifeChart.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LifeChart.Infrastructure.Repositories;

public class MedicationRepository : IMedicationRepository
{
    private readonly LifeChartDbContext _context;

    public MedicationRepository(LifeChartDbContext context)
        => _context = context;

    public async Task<IEnumerable<Medication>> GetAllActiveAsync()
        => await _context.Medications
            .Where(m => m.Active)
            .OrderBy(m => m.Name)
            .ToListAsync();

    public async Task<Medication?> GetByIdAsync(int id)
        => await _context.Medications.FirstOrDefaultAsync(m => m.Id == id);

    public async Task SaveAsync(Medication medication)
    {
        if (medication.Id == 0)
            _context.Medications.Add(medication);
        else
            _context.Medications.Update(medication);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var medication = await GetByIdAsync(id);
        if (medication is null) return;
        medication.Deactivate();
        await _context.SaveChangesAsync();
    }
}
