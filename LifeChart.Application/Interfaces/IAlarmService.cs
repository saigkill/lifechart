using LifeChart.Application.DTOs;

namespace LifeChart.Application.Interfaces;

public interface IAlarmService
{
    Task ScheduleAsync(IEnumerable<MedicationDto> medications);
    Task CancelAllAsync();
    Task<bool> RequestPermissionsAsync();
    bool HasRequiredPermissions { get; }
}
