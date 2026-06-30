using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskTracker.Application.Users.UpdateUser;
using TaskTracker.Maui.Common.Responses;
using TaskTracker.Maui.Features.Base;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Features.Users.ViewModels;

[QueryProperty(nameof(UserId), "Id")]
public partial class EditUserViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty]
    private string userId = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;
    [ObservableProperty]
    private string emailError = string.Empty;

    [ObservableProperty]
    private string firstName = string.Empty;
    [ObservableProperty]
    private string firstNameError = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;
    [ObservableProperty]
    private string lastNameError = string.Empty;

    [ObservableProperty]
    private string role = string.Empty;
    [ObservableProperty]
    private string roleError = string.Empty;

    public List<string> Roles { get; } = RoleProvider.All.ToList();
    protected override void ApplyFieldErrors(List<ApiError> errors)
    {
        EmailError = "";
        FirstNameError = "";
        LastNameError = "";
        RoleError = "";

        foreach (var e in errors)
        {
            switch (e.Field)
            {
                case "Email": EmailError = e.Error; break;
                case "FirstName": FirstNameError = e.Error; break;
                case "LastName": LastNameError = e.Error; break;
                case "Role": RoleError = e.Error; break;
            }
        }
    }
    public EditUserViewModel(ApiService api)
    {
        _api = api;
    }

    partial void OnUserIdChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Load(value);
        });
    }

    private async Task Load(string id)
    {
        var result = await _api.GetUserByIdAsync(id);

        await HandleResult(result, user =>
        {
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Role = user.Role;
        });
    }

    [RelayCommand]
    private async Task Save()
    {
        var result = await _api.UpdateUserAsync(new UpdateUserRequest
        {
            UserId = UserId,
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            Role = Role
        });

        await HandleResult(result, async () =>
        {
            await Shell.Current.DisplayAlertAsync("Success", "Updated", "OK");
            await Shell.Current.GoToAsync("..");
        });
    }
}