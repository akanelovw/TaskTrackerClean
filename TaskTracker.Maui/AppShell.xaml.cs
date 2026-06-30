using TaskTracker.Application.Common;
using TaskTracker.Maui.Features.Auth.Views;
using TaskTracker.Maui.Features.Documents.Popups;
using TaskTracker.Maui.Features.Projects.Views;
using TaskTracker.Maui.Features.Users.Popups;
using TaskTracker.Maui.Features.Users.Views;
using TaskTracker.Maui.Features.WorkItems.Views;
using TaskTracker.Maui.Interfaces;
using TaskTracker.Maui.Popups;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui;

public partial class AppShell : Shell
{
    private readonly ICurrentUserService _currentUser;
    private readonly TokenStore _tokenStore;

    public bool IsAdmin => _currentUser.IsInRole(Roles.Admin);

    public AppShell(
        ICurrentUserService currentUser,
        TokenStore tokenStore)
    {
        InitializeComponent();

        _currentUser = currentUser;
        _tokenStore = tokenStore;

        BindingContext = this;

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(ProjectsPage), typeof(ProjectsPage));
        Routing.RegisterRoute(nameof(ProjectDetailsPage), typeof(ProjectDetailsPage));
        Routing.RegisterRoute(nameof(CreateProjectPage), typeof(CreateProjectPage));
        Routing.RegisterRoute(nameof(EditProjectPage), typeof(EditProjectPage));
        Routing.RegisterRoute(nameof(CreateWorkItemPage), typeof(CreateWorkItemPage));
        Routing.RegisterRoute(nameof(EditWorkItemPopup), typeof(EditWorkItemPopup));
        Routing.RegisterRoute(nameof(DocumentsPopup), typeof(DocumentsPopup));
        Routing.RegisterRoute(nameof(UsersPage), typeof(UsersPage));
        Routing.RegisterRoute(nameof(UserDetailsPage), typeof(UserDetailsPage));
        Routing.RegisterRoute(nameof(CreateUserPage), typeof(CreateUserPage));
        Routing.RegisterRoute(nameof(EditUserPage), typeof(EditUserPage));
        Routing.RegisterRoute(nameof(UserPickerPopup), typeof(UserPickerPopup));
    }
    public void RefreshRole()
    {
        OnPropertyChanged(nameof(IsAdmin));
    }

    private async void LogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync(
            "Logout",
            "Are you sure you want to log out?",
            "Logout",
            "Cancel");

        if (!confirm)
            return;

        _tokenStore.Set(string.Empty);

        RefreshRole();

        FlyoutIsPresented = false;

        await Current.GoToAsync("//login");
    }
}