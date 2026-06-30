using TaskTracker.Maui.Features.Projects.ViewModels;

namespace TaskTracker.Maui.Features.Projects.Views;

public partial class ProjectsPage : ContentPage
{
    private readonly ProjectsViewModel _vm;

    public ProjectsPage(ProjectsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.Load();
    }

    private async void OnReload(object sender, EventArgs e)
    {
        await _vm.Load();
    }
    private async void OnCreateProject(
    object sender,
    EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(CreateProjectPage));
    }
}