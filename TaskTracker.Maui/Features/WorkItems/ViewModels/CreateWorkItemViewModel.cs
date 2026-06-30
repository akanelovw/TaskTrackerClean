using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskTracker.Application.WorkItems.CreateWorkItem;
using TaskTracker.Domain.Enums;
using TaskTracker.Maui.Common.Responses;
using TaskTracker.Maui.Features.Base;
using TaskTracker.Maui.Features.Projects.Views;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Features.WorkItems.ViewModels;

[QueryProperty(nameof(ProjectId), nameof(ProjectId))]
public partial class CreateWorkItemViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty]
    private int projectId;

    [ObservableProperty]
    private string title = string.Empty;
    [ObservableProperty]
    private string titleError = string.Empty;

    [ObservableProperty]
    private string comment = string.Empty;
    [ObservableProperty]
    private string commentError = string.Empty;

    [ObservableProperty]
    private WorkItemStatus status = WorkItemStatus.ToDo;

    [ObservableProperty]
    private WorkItemPriority priority = WorkItemPriority.Medium;

    [ObservableProperty]
    private string? assignedUserId;

    public List<WorkItemPriority> Priorities { get; } =
        Enum.GetValues<WorkItemPriority>().ToList();

    public List<WorkItemStatus> Statuses { get; } =
        Enum.GetValues<WorkItemStatus>().ToList();
    protected override void ApplyFieldErrors(List<ApiError> errors)
    {
        TitleError = "";
        CommentError = "";

        foreach (var e in errors)
        {
            switch (e.Field)
            {
                case "Title": TitleError = e.Error; break;
                case "Comment": CommentError = e.Error; break;
            }
        }
    }
    public CreateWorkItemViewModel(ApiService api)
    {
        _api = api;
    }

    [RelayCommand]
    private async Task Create()
    {
        var result = await _api.CreateWorkItemAsync(new CreateWorkItemRequest
        {
            Title = Title,
            Comment = Comment,
            ProjectId = ProjectId,
            Status = Status,
            Priority = Priority,
            AssignedUserId = AssignedUserId
        });

        await HandleResult(result, async _ =>
        {
            await Shell.Current.GoToAsync($"{nameof(ProjectDetailsPage)}?Id={projectId}");
        });
    }
}