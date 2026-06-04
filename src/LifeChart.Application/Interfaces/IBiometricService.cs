namespace LifeChart.Application.Interfaces;

public record BiometricResult(bool Success, string? ErrorMessage);

public interface IBiometricService
{
    Task<bool> IsAvailableAsync();
    Task<BiometricResult> AuthenticateAsync();
}
