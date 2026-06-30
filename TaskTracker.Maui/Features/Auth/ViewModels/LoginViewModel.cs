using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Features.Auth.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _auth;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string error = string.Empty;

    public LoginViewModel(AuthService auth)
    {
        _auth = auth;
    }

    [RelayCommand]
    private async Task Login()
    {
        try
        {
            await _auth.LoginAsync(email, password);

            if (Shell.Current is AppShell appShell)
                appShell.RefreshRole();

            await Shell.Current.GoToAsync("//projects");
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }
}