using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskTracker.Application.Users.CreateUser;
using TaskTracker.Maui.Common.Responses;
using TaskTracker.Maui.Features.Base;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Features.Users.ViewModels;

public partial class CreateUserViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty] 
    private string email = string.Empty;
    [ObservableProperty] 
    private string emailError = string.Empty;

    [ObservableProperty] 
    private string password = string.Empty;
    [ObservableProperty] 
    private string passwordError = string.Empty;

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

    public CreateUserViewModel(ApiService api)
    {
        _api = api;
        Role = Roles.FirstOrDefault()!;
    }
    protected override void ApplyFieldErrors(List<ApiError> errors)
    {
        EmailError = "";
        PasswordError = "";
        FirstNameError = "";
        LastNameError = "";
        RoleError = "";

        foreach (var e in errors)
        {
            switch (e.Field)
            {
                case "Email": EmailError = e.Error; break;
                case "Password": PasswordError = e.Error; break;
                case "FirstName": FirstNameError = e.Error; break;
                case "LastName": LastNameError = e.Error; break;
                case "Role": RoleError = e.Error; break;
            }
        }
    }

    [RelayCommand]
    private async Task Create()
    {
        var result = await _api.CreateUserAsync(new CreateUserRequest
        {
            Email = Email,
            Password = Password,
            FirstName = FirstName,
            LastName = LastName,
            Role = Role
        });

        await HandleResult(result, async () =>
        {
            await Shell.Current.DisplayAlertAsync("Success", "User created", "OK");
            await Shell.Current.GoToAsync("..");
        });
    }
}