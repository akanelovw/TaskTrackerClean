using TaskTracker.Maui.Features.Users.ViewModels;

namespace TaskTracker.Maui.Features.Users.Views;

public partial class CreateUserPage : ContentPage
{
    public CreateUserPage(CreateUserViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}