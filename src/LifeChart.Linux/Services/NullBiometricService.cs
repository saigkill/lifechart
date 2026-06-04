using LifeChart.Application.Interfaces;

namespace LifeChart.Linux.Services;

public class NullBiometricService : IBiometricService
{
    public Task<bool> IsAvailableAsync() => Task.FromResult(false);

    public Task<BiometricResult> AuthenticateAsync() =>
        Task.FromResult(new BiometricResult(true, null));
}
