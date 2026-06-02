using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeChart.Application.Interfaces;

namespace LifeChart.ViewModels;

public partial class LockViewModel : ObservableObject
{
    private readonly IBiometricService _biometricService;

    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isAuthenticating;

    public LockViewModel(IBiometricService biometricService)
        => _biometricService = biometricService;

    public async Task TryAuthenticateAsync()
    {
        IsAuthenticating = true;
        ErrorMessage = null;

        try
        {
            var result = await _biometricService.AuthenticateAsync();

            if (result.Success)
                AuthenticationSucceeded?.Invoke(this, EventArgs.Empty);
            else
                ErrorMessage = result.ErrorMessage ?? "Authentifizierung fehlgeschlagen.";
        }
        finally
        {
            IsAuthenticating = false;
        }
    }

    [RelayCommand]
    private async Task RetryAsync() => await TryAuthenticateAsync();

    public event EventHandler? AuthenticationSucceeded;
}
