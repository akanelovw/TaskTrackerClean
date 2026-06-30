using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TaskTracker.Application.Projects.GetProjectDetails;
using TaskTracker.Application.Users.GetUsers;
using TaskTracker.Domain.Enums;
using TaskTracker.Maui.Features.Base;
using TaskTracker.Maui.Features.Documents.Popups;
using TaskTracker.Maui.Features.Projects.Views;
using TaskTracker.Maui.Features.Users.Popups;
using TaskTracker.Maui.Features.WorkItems.ViewModels;
using TaskTracker.Maui.Features.WorkItems.Views;
using TaskTracker.Maui.Interfaces;
using TaskTracker.Maui.Popups;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Features.Projects.ViewModels;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class ProjectDetailsViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly ICurrentUserService _currentUser;
    private const int MembersPageSize = 5;
    private List<ProjectMemberResponse> _allMembers = new();

    public WorkItemsViewModel WorkItemsVM { get; }

    public ProjectDetailsViewModel(
        ApiService api,
        WorkItemsViewModel workItemsVM,
        ICurrentUserService currentUser)
    {
        _api = api;
        WorkItemsVM = workItemsVM;
        _currentUser = currentUser;
    }

    // ================= PROJECT =================

    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string title = "";

    [ObservableProperty]
    private string customerCompany = "";

    [ObservableProperty]
    private string executorCompany = "";

    [ObservableProperty]
    private DateTime startTime;

    [ObservableProperty]
    private DateTime endTime;

    [ObservableProperty]
    private string status = "";

    [ObservableProperty]
    private string priority = "";

    [ObservableProperty]
    private string managerName = "";

    [ObservableProperty]
    private string? managerUserId;

    [ObservableProperty]
    private ObservableCollection<ProjectMemberResponse> members = new();

    [ObservableProperty]
    private int membersPage = 1;

    [ObservableProperty]
    private bool hasNextMembersPage;

    [ObservableProperty]
    private bool hasPreviousMembersPage;

    public string MembersPageLabel => $"Page {MembersPage} / {Math.Max(1, (int)Math.Ceiling(_allMembers.Count / (double)MembersPageSize))}";

    // ================= PERMISSIONS =================

    public bool IsAdmin => _currentUser.IsInRole("Admin");
    public bool IsChiefProjectManager => _currentUser.IsInRole("ChiefProjectManager");
    public bool IsProjectManager => _currentUser.IsInRole("ProjectManager");
    public bool IsWorker => _currentUser.IsInRole("Worker");

    public bool IsCurrentUserManagerOfThisProject =>
        !string.IsNullOrEmpty(ManagerUserId) &&
        ManagerUserId == _currentUser.UserId;

    public bool CanEditProject => IsAdmin || IsChiefProjectManager;
    public bool CanDeleteProject => IsAdmin || IsChiefProjectManager;
    public bool CanChangeStatus => IsAdmin || IsChiefProjectManager;
    public bool CanChangePriority => IsAdmin || IsChiefProjectManager;
    public bool CanAssignManager => IsAdmin || IsChiefProjectManager;
    public bool CanAddMember => IsAdmin || IsChiefProjectManager;
    public bool CanRemoveMember => IsAdmin || IsChiefProjectManager;

    public bool CanCreateWorkItem =>
        IsAdmin || IsChiefProjectManager ||
        (IsProjectManager && IsCurrentUserManagerOfThisProject);

    public bool CanUploadDocuments =>
        IsAdmin || IsChiefProjectManager ||
        (IsProjectManager && IsCurrentUserManagerOfThisProject) ||
        IsWorker;

    partial void OnManagerUserIdChanged(string? value)
    {
        OnPropertyChanged(nameof(IsCurrentUserManagerOfThisProject));
        OnPropertyChanged(nameof(CanCreateWorkItem));
        OnPropertyChanged(nameof(CanUploadDocuments));
    }

    // ================= LOAD =================

    partial void OnIdChanged(int value)
    {
        _ = LoadProject(value);
    }
    partial void OnStatusChanged(string value)
    {
        OnPropertyChanged(nameof(StatusColor));
    }

    partial void OnPriorityChanged(string value)
    {
        OnPropertyChanged(nameof(PriorityColor));
    }
    public Color StatusColor =>
    Status switch
    {
        "Active" => Colors.DodgerBlue,
        "Completed" => Colors.Green,
        "Archived" => Colors.Gray,
        _ => Colors.DarkGray
    };

    public Color PriorityColor =>
        Priority switch
        {
            "Low" => Colors.Green,
            "Medium" => Colors.Goldenrod,
            "High" => Colors.Orange,
            "Critical" => Colors.Red,
            _ => Colors.DarkGray
        };

    private async Task LoadProject(int projectId)
    {
        var result = await _api.GetProjectAsync(projectId);

        if (!result.Success)
        {
            await HandleResult(result, _ => { });
            return;
        }

        var project = result.Data!;

        Title = project.Title;
        CustomerCompany = project.CustomerCompany;
        ExecutorCompany = project.ExecutorCompany;
        StartTime = project.StartTime;
        EndTime = project.EndTime;
        Status = project.Status;
        Priority = project.Priority;
        ManagerUserId = project.ManagerUserId;
        ManagerName = project.ManagerName ?? "";

        _allMembers = project.Members.ToList();
        MembersPage = 1;
        ApplyMembersPage();

        await WorkItemsVM.Load(projectId);
        WorkItemsVM.SetProjectMembers(project.Members);
    }
    private async Task Refresh()
    {
        await LoadProject(Id);
    }
    public async Task Reload()
    {
        System.Diagnostics.Debug.WriteLine("RELOAD");
        await LoadProject(Id);
    }
    private void ApplyMembersPage()
    {
        Members.Clear();

        foreach (var m in _allMembers
            .Skip((MembersPage - 1) * MembersPageSize)
            .Take(MembersPageSize))
        {
            Members.Add(m);
        }

        var totalPages = Math.Max(1, (int)Math.Ceiling(_allMembers.Count / (double)MembersPageSize));
        HasNextMembersPage = MembersPage < totalPages;
        HasPreviousMembersPage = MembersPage > 1;
        OnPropertyChanged(nameof(MembersPageLabel));
    }

    // ================= DELETE =================

    [RelayCommand]
    private async Task Delete()
    {
        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete Project",
            $"Delete '{Title}'?",
            "Delete",
            "Cancel");

        if (!confirm)
            return;

        var result = await _api.DeleteProjectAsync(Id);

        await HandleResult(result, async () =>
        {
            await Shell.Current.GoToAsync(
            $"{nameof(ProjectsPage)}");
        });
    }

    // ================= STATUS =================

    [RelayCommand]
    private async Task ChangeStatus()
    {
        string result = await Shell.Current.DisplayActionSheetAsync(
            "Change status",
            "Cancel",
            null,
            "Active",
            "Completed",
            "Archived");

        if (result == "Cancel")
            return;

        if (!Enum.TryParse<ProjectStatus>(result, true, out var status))
            return;

        var apiResult = await _api.ChangeProjectStatusAsync(Id, status);

        await HandleResult(apiResult, async () =>
        {
            await Refresh();
        });
    }

    // ================= PRIORITY =================

    [RelayCommand]
    private async Task ChangePriority()
    {
        string result = await Shell.Current.DisplayActionSheetAsync(
            "Change priority",
            "Cancel",
            null,
            "Low",
            "Medium",
            "High",
            "Critical");

        if (result == "Cancel")
            return;

        if (!Enum.TryParse<ProjectPriority>(result, true, out var priority))
            return;

        var apiResult = await _api.ChangeProjectPriorityAsync(Id, priority);

        await HandleResult(apiResult, async () =>
        {
            await Refresh();
        });
    }

    // ================= ADD MEMBER =================

    [RelayCommand]
    private async Task AddMember()
    {
        var result = await _api.GetUsersAsync();

        await HandleResult(result, async users =>
        {
            var items = users.Select(u => new UserPickerItem
            {
                Id = u.Id,
                FullName = $"{u.LastName} {u.FirstName}",
                Role = u.Role ?? ""
            });

            var selected = await UserPickerPopupExtensions.ShowUserPickerAsync(
                "Add member", items);

            if (selected == null)
                return;

            var addResult = await _api.AddMemberAsync(Id, selected.Id);

            await HandleResult(addResult, async () =>
            {
                await Refresh();
            });
        });
    }

    // ================= REMOVE MEMBER =================

    [RelayCommand]
    private async Task RemoveMember(ProjectMemberResponse member)
    {
        if (member is null)
            return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Remove member",
            $"Remove {member.FullName}?",
            "Remove",
            "Cancel");

        if (!confirm)
            return;

        var result = await _api.RemoveMemberAsync(Id, member.UserId);

        await HandleResult(result, async () =>
        {
            await Refresh();
        });
    }

    private static readonly string[] ManagerRoles =
     {
        "Admin",
        "ChiefProjectManager",
        "ProjectManager"
    };

    [RelayCommand]
    private async Task AssignManager()
    {
        try
        {
            var users = new List<GetUsersResponse>();

            foreach (var role in ManagerRoles)
            {
                var result = await _api.GetUsersAsync(role);

                await HandleResult(result, data =>
                {
                    if (data != null)
                        users.AddRange(data);
                });
            }

            var uniqueUsers = users
                .GroupBy(x => x.Id)
                .Select(x => x.First())
                .ToList();

            if (uniqueUsers.Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("Error", "No users found", "OK");
                return;
            }

            var items = uniqueUsers.Select(u => new UserPickerItem
            {
                Id = u.Id,
                FullName = $"{u.LastName} {u.FirstName}",
                Role = u.Role ?? ""
            });

            var selected = await UserPickerPopupExtensions.ShowUserPickerAsync(
                "Select manager", items);

            if (selected == null)
                return;

            var apiResult = await _api.AssignManagerAsync(Id, selected.Id);

            await HandleResult(apiResult, async () =>
            {
                await Refresh();
            });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Unexpected error", ex.Message, "OK");
        }
    }

    // ================= MEMBERS PAGINATION =================

    [RelayCommand]
    private void NextMembersPage()
    {
        if (!HasNextMembersPage) return;
        MembersPage++;
        ApplyMembersPage();
    }

    [RelayCommand]
    private void PreviousMembersPage()
    {
        if (!HasPreviousMembersPage) return;
        MembersPage--;
        ApplyMembersPage();
    }

    // ================= NAV =================

    [RelayCommand]
    private async Task CreateWorkItem()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(CreateWorkItemPage)}?ProjectId={Id}");
    }

    [RelayCommand]
    private async Task OpenDocuments()
    {
        var popup = new DocumentsPopup(Id, _api);
        await Shell.Current.CurrentPage.ShowPopupAsync(
            popup,
            new PopupOptions
            {
                Shape = null,
                Shadow = null,
                PageOverlayColor = Colors.Black.WithAlpha(0.5f)
            });
    }

    [RelayCommand]
    private async Task Edit()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(EditProjectPage)}?Id={Id}");
    }
}