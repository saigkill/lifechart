using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;

namespace LifeChart.Linux.Services;

public class NullAlarmService : IAlarmService
{
    public Task ScheduleAsync(IEnumerable<MedicationDto> medications) => Task.CompletedTask;
    public Task CancelAllAsync() => Task.CompletedTask;
    public Task<bool> RequestPermissionsAsync() => Task.FromResult(true);
    public bool HasRequiredPermissions => true;
}
