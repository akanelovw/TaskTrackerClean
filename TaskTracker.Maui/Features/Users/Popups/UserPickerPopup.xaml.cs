using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace TaskTracker.Maui.Features.Users.Popups;

public partial class UserPickerPopup : Popup
{
    private readonly TaskCompletionSource<UserPickerItem?> _tcs = new();
    public Task<UserPickerItem?> Result => _tcs.Task;

    public UserPickerPopup(string title, IEnumerable<UserPickerItem> users)
    {
        InitializeComponent();

        var vm = new UserPickerViewModel(users, async item =>
        {
            _tcs.TrySetResult(item);
            await CloseAsync();
        });

        TitleLabel.Text = title;
        BindingContext = vm;
        UsersCollection.SetBinding(CollectionView.ItemsSourceProperty,
            new Binding(nameof(UserPickerViewModel.PagedUsers)));
    }

    private async void CancelClicked(object sender, EventArgs e)
    {
        _tcs.TrySetResult(null);
        await CloseAsync();
    }
}

// ================= ITEM =================

public class UserPickerItem
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Role { get; set; } = "";
    public string Initials => GetInitials(FullName);

    private static string GetInitials(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => "?",
            1 => parts[0][..1].ToUpper(),
            _ => $"{parts[0][0]}{parts[1][0]}".ToUpper()
        };
    }
}

// ================= VIEWMODEL =================

public partial class UserPickerViewModel : ObservableObject
{
    private const int PageSize = 5;
    private readonly List<UserPickerItem> _allUsers;
    private readonly Func<UserPickerItem, Task> _onSelected;

    [ObservableProperty]
    private ObservableCollection<UserPickerItem> pagedUsers = [];

    [ObservableProperty]
    private int page = 1;

    [ObservableProperty]
    private bool hasNextPage;

    [ObservableProperty]
    private bool hasPreviousPage;

    public string PageLabel => $"Page {Page} / {TotalPages}";
    private int TotalPages => Math.Max(1, (int)Math.Ceiling(_allUsers.Count / (double)PageSize));

    public UserPickerViewModel(IEnumerable<UserPickerItem> users, Func<UserPickerItem, Task> onSelected)
    {
        _allUsers = users.ToList();
        _onSelected = onSelected;
        ApplyPage();
    }

    private void ApplyPage()
    {
        PagedUsers.Clear();

        foreach (var u in _allUsers
            .Skip((Page - 1) * PageSize)
            .Take(PageSize))
        {
            PagedUsers.Add(u);
        }

        HasNextPage = Page < TotalPages;
        HasPreviousPage = Page > 1;
        OnPropertyChanged(nameof(PageLabel));
    }

    [RelayCommand]
    private void NextPage()
    {
        if (!HasNextPage) return;
        Page++;
        ApplyPage();
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (!HasPreviousPage) return;
        Page--;
        ApplyPage();
    }

    [RelayCommand]
    private async Task Select(UserPickerItem item)
    {
        if (item is null) return;
        await _onSelected(item);
    }
}