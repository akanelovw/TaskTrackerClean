using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskTracker.Application.Projects.CreateProject;
using TaskTracker.Domain.Enums;
using TaskTracker.Maui.Common.Responses;
using TaskTracker.Maui.Features.Base;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Features.Projects.ViewModels;

public partial class CreateProjectViewModel : BaseViewModel
{
    private readonly ApiService _api;

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
    private DateTime startTime = DateTime.Today;
    [ObservableProperty]
    private DateTime endTime = DateTime.Today.AddDays(30);

    [ObservableProperty]
    private ProjectPriority priority = ProjectPriority.Medium;

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
    public CreateProjectViewModel(ApiService api)
    {
        _api = api;
    }

    partial void OnPriorityChanged(ProjectPriority value)
    {
        OnPropertyChanged(nameof(PriorityColor));
    }

    [RelayCommand]
    private async Task Create()
    {
        var result = await _api.CreateProjectAsync(new CreateProjectRequest
        {
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