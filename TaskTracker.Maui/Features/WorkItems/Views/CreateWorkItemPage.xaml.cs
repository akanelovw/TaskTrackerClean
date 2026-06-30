using TaskTracker.Maui.Features.WorkItems.ViewModels;

namespace TaskTracker.Maui.Features.WorkItems.Views;

public partial class CreateWorkItemPage : ContentPage
{
    public CreateWorkItemPage(CreateWorkItemViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}