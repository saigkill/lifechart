using LifeChart.Application.Interfaces;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace LifeChart.Services;

public class PluginFingerprintBiometricService : IBiometricService
{
    private static readonly AuthenticationRequestConfiguration Config = new(
        "LifeChart entsperren",
        "Identität mit Biometrie bestätigen")
    {
        ConfirmationRequired = false,
        FallbackTitle = "PIN verwenden"
    };

    public async Task<bool> IsAvailableAsync()
    {
        var availability = await CrossFingerprint.Current.GetAvailabilityAsync();
        return availability == FingerprintAvailability.Available;
    }

    public async Task<BiometricResult> AuthenticateAsync()
    {
        var result = await CrossFingerprint.Current.AuthenticateAsync(Config);
        return new BiometricResult(
            result.Authenticated,
            result.Authenticated ? null : result.ErrorMessage);
    }
}
