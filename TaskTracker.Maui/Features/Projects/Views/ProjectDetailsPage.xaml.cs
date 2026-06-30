using TaskTracker.Maui.Features.Projects.ViewModels;

namespace TaskTracker.Maui.Features.Projects.Views;

public partial class ProjectDetailsPage : ContentPage
{
    public ProjectDetailsPage(ProjectDetailsViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ProjectDetailsViewModel vm)
        {
            _ = vm.Reload();
        }
    }
}