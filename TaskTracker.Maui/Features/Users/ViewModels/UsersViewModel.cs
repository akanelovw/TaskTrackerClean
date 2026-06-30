using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TaskTracker.Application.Users.GetUsers;
using TaskTracker.Maui.Features.Base;
using TaskTracker.Maui.Features.Users.Views;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Features.Users.ViewModels;

public partial class UsersViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private const int PageSize = 10;

    public ObservableCollection<GetUsersResponse> Users { get; } = [];

    [ObservableProperty]
    private int page = 1;

    [ObservableProperty]
    private bool hasNextPage;

    [ObservableProperty]
    private bool hasPreviousPage;

    public UsersViewModel(ApiService api)
    {
        _api = api;
    }

    [RelayCommand]
    public async Task Load()
    {
        await LoadPage(1);
    }

    private async Task LoadPage(int page)
    {
        var result = await _api.GetUsersAsync(page: page, pageSize: PageSize);

        await HandleResult(result, data =>
        {
            Users.Clear();

            foreach (var u in data)
                Users.Add(u);

            Page = page;
            HasNextPage = data.Count == PageSize;
            HasPreviousPage = page > 1;
        });
    }

    [RelayCommand]
    private async Task NextPage()
    {
        if (!HasNextPage)
            return;

        await LoadPage(Page + 1);
    }

    [RelayCommand]
    private async Task PreviousPage()
    {
        if (!HasPreviousPage)
            return;

        await LoadPage(Page - 1);
    }

    [RelayCommand]
    public async Task Delete(GetUsersResponse user)
    {
        if (user is null)
            return;

        bool confirm =
            await Shell.Current.DisplayAlertAsync(
                "Delete user",
                $"Delete {user.Email}?",
                "Delete",
                "Cancel");

        if (!confirm)
            return;

        var result = await _api.DeleteUserAsync(user.Id);

        await HandleResult(result, async () =>
        {
            await Shell.Current.DisplayAlertAsync("Success", "Deleted", "OK");
            await LoadPage(Page);
        });
    }

    [RelayCommand]
    public async Task Open(GetUsersResponse user)
    {
        if (user is null)
            return;

        await Shell.Current.GoToAsync(
            $"{nameof(UserDetailsPage)}?Id={user.Id}");
    }

    [RelayCommand]
    public async Task Create()
    {
        await Shell.Current.GoToAsync(nameof(CreateUserPage));
    }
}