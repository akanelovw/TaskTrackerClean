using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TaskTracker.Application.Projects.GetProjectDetails;
using TaskTracker.Application.WorkItems.GetWorkItems;
using TaskTracker.Application.WorkItems.UpdateWorkItem;
using TaskTracker.Domain.Enums;
using TaskTracker.Maui.Common.Responses;
using TaskTracker.Maui.Features.Base;
using TaskTracker.Maui.Features.Users.Popups;
using TaskTracker.Maui.Interfaces;
using TaskTracker.Maui.Popups;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Features.WorkItems.ViewModels;

public partial class WorkItemsViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly ICurrentUserService _currentUser;
    private const int PageSize = 5;

    private int _projectId;
    private List<ProjectMemberResponse> _projectMembers = new();

    [ObservableProperty]
    private string title = string.Empty;
    [ObservableProperty]
    private string titleError = string.Empty;

    [ObservableProperty]
    private string comment = string.Empty;
    [ObservableProperty]
    private string commentError = string.Empty;

    [ObservableProperty]
    private ObservableCollection<GetWorkItemsResponse> workItems = [];

    [ObservableProperty]
    private int page = 1;

    [ObservableProperty]
    private bool hasNextPage;

    [ObservableProperty]
    private bool hasPreviousPage;

    // ================= PERMISSIONS =================

    public bool IsAdmin => _currentUser.IsInRole("Admin");
    public bool IsChiefProjectManager => _currentUser.IsInRole("ChiefProjectManager");
    public bool IsProjectManager => _currentUser.IsInRole("ProjectManager");
    public bool IsWorker => _currentUser.IsInRole("Worker");

    public bool CanManageWorkItems => IsAdmin || IsChiefProjectManager || IsProjectManager;
    public bool CanChangeTaskStatus => IsAdmin || IsChiefProjectManager || IsProjectManager || IsWorker;

    protected override void ApplyFieldErrors(List<ApiError> errors)
    {
        TitleError = "";
        CommentError = "";

        foreach (var e in errors)
        {
            switch (e.Field)
            {
                case "Email": TitleError = e.Error; break;
                case "Password": CommentError = e.Error; break;
            }
        }
    }

    public WorkItemsViewModel(ApiService api, ICurrentUserService currentUser)
    {
        _api = api;
        _currentUser = currentUser;
    }

    // ================= LOAD =================

    public async Task Load(int projectId)
    {
        _projectId = projectId;
        await LoadPage(1);
    }
    public void SetProjectMembers(List<ProjectMemberResponse> members)
    {
        _projectMembers = members;
    }

    private async Task LoadPage(int page)
    {
        var result = await _api.GetWorkItemsAsync(_projectId, page, PageSize);

        await HandleResult(result, data =>
        {
            WorkItems.Clear();

            foreach (var item in data)
                WorkItems.Add(item);

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

    // ================= COMMANDS =================

    [RelayCommand]
    private async Task OpenTask(GetWorkItemsResponse workItem)
    {
        if (workItem is null) return;

        var options = new List<string>();

        if (CanManageWorkItems)
        {
            options.Add("Edit");
            options.Add("Assign User");
        }

        if (CanChangeTaskStatus)
        {
            options.Add("Change Status");
        }

        if (CanManageWorkItems)
        {
            options.Add("Change Priority");
            options.Add("Delete");
        }

        if (options.Count == 0)
            return;

        var result = await Shell.Current.DisplayActionSheetAsync(
            workItem.Title,
            "Cancel",
            null,
            options.ToArray());

        switch (result)
        {
            case "Edit":
                await EditTask(workItem);
                break;

            case "Assign User":
                await AssignTaskUser(workItem);
                break;

            case "Change Status":
                await ChangeTaskStatus(workItem);
                break;

            case "Change Priority":
                await ChangeTaskPriority(workItem);
                break;

            case "Delete":
                await DeleteTask(workItem);
                break;
        }
    }

    private async Task AssignTaskUser(GetWorkItemsResponse workItem)
    {
        if (!CanManageWorkItems)
            return;

        if (_projectMembers == null || !_projectMembers.Any())
        {
            await Shell.Current.DisplayAlertAsync("Error", "No members to assign", "OK");
            return;
        }

        var result = await _api.GetUsersAsync();

        await HandleResult(result, async users =>
        {
            var allowedIds = _projectMembers.Select(x => x.UserId).ToHashSet();

            var items = users
                .Where(u => allowedIds.Contains(u.Id))
                .Select(u => new UserPickerItem
                {
                    Id = u.Id,
                    FullName = $"{u.LastName} {u.FirstName}",
                    Role = u.Role ?? ""
                });

            var selected = await UserPickerPopupExtensions.ShowUserPickerAsync(
                "Assign user", items);

            if (selected == null)
                return;

            await _api.AssignWorkItemUserAsync(workItem.Id, selected.Id);
            await LoadPage(Page);
        });
    }

    private async Task ChangeTaskStatus(GetWorkItemsResponse workItem)
    {
        if (!CanChangeTaskStatus)
            return;

        string result = await Shell.Current.DisplayActionSheetAsync(
            "Change status",
            "Cancel",
            null,
            "ToDo",
            "InProgress",
            "Done");

        if (result == "Cancel") return;

        if (!Enum.TryParse<WorkItemStatus>(result, true, out var status))
            return;

        var apiResult = await _api.ChangeWorkItemStatusAsync(workItem.Id, status);

        await HandleResult(apiResult, async () =>
        {
            await LoadPage(Page);
        });
    }

    private async Task ChangeTaskPriority(GetWorkItemsResponse workItem)
    {
        if (!CanManageWorkItems)
            return;

        string result = await Shell.Current.DisplayActionSheetAsync(
            "Change priority",
            "Cancel",
            null,
            "Low",
            "Medium",
            "High",
            "Critical");

        if (result == "Cancel") return;

        if (!Enum.TryParse<WorkItemPriority>(result, true, out var priority))
            return;

        var apiResult = await _api.ChangeWorkItemPriorityAsync(workItem.Id, priority);

        await HandleResult(apiResult, async () =>
        {
            await LoadPage(Page);
        });
    }

    private async Task DeleteTask(GetWorkItemsResponse workItem)
    {
        if (!CanManageWorkItems)
            return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete task",
            $"Delete '{workItem.Title}'?",
            "Delete",
            "Cancel");

        if (!confirm) return;

        var result = await _api.DeleteWorkItemAsync(workItem.Id);

        await HandleResult(result, async () =>
        {
            var targetPage = (WorkItems.Count == 1 && Page > 1) ? Page - 1 : Page;
            await LoadPage(targetPage);
        });
    }

    private async Task EditTask(GetWorkItemsResponse workItem)
    {
        if (!CanManageWorkItems)
            return;

        var popup = new EditWorkItemPopup(
            workItem.Title,
            workItem.Comment ?? "");

        await Shell.Current.CurrentPage.ShowPopupAsync(
            popup,
            new PopupOptions
            {
                Shape = null,
                Shadow = null,
                PageOverlayColor = Colors.Black.WithAlpha(0.5f)
            });

        var popupResult = await popup.Result;

        if (popupResult == null)
            return;

        var result = await _api.UpdateWorkItemAsync(new UpdateWorkItemRequest
        {
            Id = workItem.Id,
            Title = popupResult.Title,
            Comment = popupResult.Comment
        });

        if (result.FieldErrors?.Any() == true)
        {
            popup.ApplyFieldErrors(result.FieldErrors);
            return;
        }

        await HandleResult(result, async () =>
        {
            await LoadPage(Page);
        });
    }
}