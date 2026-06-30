using TaskTracker.Maui.Features.Users.ViewModels;

namespace TaskTracker.Maui.Features.Users.Views;

public partial class UserDetailsPage : ContentPage
{
    public UserDetailsPage(UserDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}