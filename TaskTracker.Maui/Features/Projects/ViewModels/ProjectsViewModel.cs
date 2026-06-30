using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TaskTracker.Application.Projects.GetProjectsList;
using TaskTracker.Maui.Features.Base;
using TaskTracker.Maui.Features.Projects.Views;
using TaskTracker.Maui.Interfaces;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Features.Projects.ViewModels;

public partial class ProjectsViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly ICurrentUserService _currentUser;
    private const int PageSize = 10;

    [ObservableProperty]
    private ObservableCollection<GetProjectsListResponse> projects = new();

    [ObservableProperty]
    private int page = 1;

    [ObservableProperty]
    private bool hasNextPage;

    [ObservableProperty]
    private bool hasPreviousPage;

    public ProjectsViewModel(ApiService api, ICurrentUserService currentUser)
    {
        _api = api;
        _currentUser = currentUser;
    }
    // ================= PERMISSIONS =================
    public bool IsAdmin => _currentUser.IsInRole("Admin");
    public bool IsChiefProjectManager => _currentUser.IsInRole("ChiefProjectManager");
    public bool IsProjectManager => _currentUser.IsInRole("ProjectManager");
    public bool IsWorker => _currentUser.IsInRole("Worker");

    public bool CanCreateProject => IsAdmin || IsChiefProjectManager;


    // ================= LOAD =================
    [RelayCommand]
    public async Task Load()
    {
        await LoadPage(1);
    }

    private async Task LoadPage(int page)
    {
        var result = await _api.GetProjectsAsync(page, PageSize);

        await HandleResult(result, data =>
        {
            Projects.Clear();

            foreach (var item in data)
                Projects.Add(item);

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

    // ================= NAVIGATION =================
    [RelayCommand]
    public async Task OpenProject(GetProjectsListResponse project)
    {
        if (project is null)
            return;

        await Shell.Current.GoToAsync(
            $"{nameof(ProjectDetailsPage)}?Id={project.Id}");
    }
}