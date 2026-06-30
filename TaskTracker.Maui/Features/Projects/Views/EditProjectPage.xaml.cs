using TaskTracker.Maui.Features.Projects.ViewModels;

namespace TaskTracker.Maui.Features.Projects.Views;

public partial class EditProjectPage : ContentPage
{
    public EditProjectPage(EditProjectViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }
}