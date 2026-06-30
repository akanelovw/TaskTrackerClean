using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskTracker.Application.Users.GetUserById;
using TaskTracker.Maui.Features.Base;
using TaskTracker.Maui.Features.Users.Views;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Features.Users.ViewModels;

[QueryProperty(nameof(Id), "Id")]
public partial class UserDetailsViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty]
    private string id;

    [ObservableProperty]
    private GetUserByIdResponse? user;

    public UserDetailsViewModel(ApiService api)
    {
        _api = api;
    }

    partial void OnIdChanged(string value)
    {
        _ = Load(value);
    }

    [RelayCommand]
    private async Task Load(string id)
    {
        var result = await _api.GetUserByIdAsync(id);

        await HandleResult(result, data =>
        {
            User = data;
        });
    }

    // ================= COMMANDS =================

    [RelayCommand]
    private async Task Edit()
    {
        if (User is null)
            return;

        await Shell.Current.GoToAsync($"{nameof(EditUserPage)}?Id={User.Id}");
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (User is null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete user",
            $"Delete {User.Email}?",
            "Delete",
            "Cancel");

        if (!confirm)
            return;

        var result = await _api.DeleteUserAsync(User.Id);

        await HandleResult(result, async _ =>
        {
            await Shell.Current.DisplayAlertAsync("Success", "Deleted", "OK");
            await Shell.Current.GoToAsync("..");
        });
    }
}