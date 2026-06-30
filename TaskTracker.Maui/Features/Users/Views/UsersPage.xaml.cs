using TaskTracker.Maui.Features.Users.ViewModels;

namespace TaskTracker.Maui.Features.Users.Views;

public partial class UsersPage : ContentPage
{
    private readonly UsersViewModel _vm;

    public UsersPage(UsersViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;

        _ = _vm.Load();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.Load();
    }
}