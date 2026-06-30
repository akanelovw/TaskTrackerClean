using TaskTracker.Maui.Features.Users.ViewModels;

namespace TaskTracker.Maui.Features.Users.Views;

public partial class EditUserPage : ContentPage
{
    public EditUserPage(EditUserViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}