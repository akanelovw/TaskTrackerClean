using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskTracker.Application.Projects.UpdateProject;
using TaskTracker.Domain.Enums;
using TaskTracker.Maui.Common.Responses;
using TaskTracker.Maui.Features.Base;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Features.Projects.ViewModels;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class EditProjectViewModel : BaseViewModel
{
    private readonly ApiService _api;

    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string title = string.Empty;
    [ObservableProperty]
    private string titleError = string.Empty;

    [ObservableProperty]
    private string customerCompany = string.Empty;
    [ObservableProperty]
    private string customerCompanyError = string.Empty;

    [ObservableProperty]
    private string executorCompany = string.Empty;
    [ObservableProperty]
    private string executorCompanyError = string.Empty;

    [ObservableProperty]
    private DateTime startTime;

    [ObservableProperty]
    private DateTime endTime;

    [ObservableProperty]
    private ProjectPriority priority;

    public List<ProjectPriority> Priorities { get; } =
        Enum.GetValues<ProjectPriority>().ToList();

    public Color PriorityColor =>
        Priority switch
        {
            ProjectPriority.Low => Colors.Green,
            ProjectPriority.Medium => Colors.Goldenrod,
            ProjectPriority.High => Colors.Orange,
            ProjectPriority.Critical => Colors.Red,
            _ => Colors.Gray
        };
    protected override void ApplyFieldErrors(List<ApiError> errors)
    {
        TitleError = "";
        CustomerCompanyError = "";
        ExecutorCompanyError = "";

        foreach (var e in errors)
        {
            switch (e.Field)
            {
                case "Title": TitleError = e.Error; break;
                case "CustomerCompany": CustomerCompanyError = e.Error; break;
                case "ExecutorCompany": ExecutorCompanyError = e.Error; break;

            }
        }
    }

    public EditProjectViewModel(ApiService api)
    {
        _api = api;
    }

    partial void OnIdChanged(int value)
    {
        _ = Load(value);
    }

    partial void OnPriorityChanged(ProjectPriority value)
    {
        OnPropertyChanged(nameof(PriorityColor));
    }

    private async Task Load(int projectId)
    {
        var result = await _api.GetProjectAsync(projectId);

        await HandleResult(result, project =>
        {
            Title = project.Title;
            CustomerCompany = project.CustomerCompany;
            ExecutorCompany = project.ExecutorCompany;
            StartTime = project.StartTime;
            EndTime = project.EndTime;

            Enum.TryParse<ProjectPriority>(project.Priority, out var p);
            Priority = p;
        });
    }

    [RelayCommand]
    private async Task Save()
    {
        var result = await _api.UpdateProjectAsync(new UpdateProjectRequest
        {
            Id = Id,
            Title = Title,
            CustomerCompany = CustomerCompany,
            ExecutorCompany = ExecutorCompany,
            StartTime = StartTime,
            EndTime = EndTime,
            Priority = Priority
        });

        await HandleResult(result, _ =>
        {
            Shell.Current.GoToAsync("..");
        });
    }
}