using TaskTracker.Maui.Features.Projects.ViewModels;

namespace TaskTracker.Maui.Features.Projects.Views;

public partial class CreateProjectPage : ContentPage
{
    public CreateProjectPage(CreateProjectViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}