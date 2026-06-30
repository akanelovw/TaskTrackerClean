using TaskTracker.Maui.Features.Auth.ViewModels;

namespace TaskTracker.Maui.Features.Auth.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}