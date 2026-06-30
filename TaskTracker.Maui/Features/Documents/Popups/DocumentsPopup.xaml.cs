using CommunityToolkit.Maui.Views;
using TaskTracker.Maui.Services;
using TaskTracker.Maui.Features.Documents.ViewModels;

namespace TaskTracker.Maui.Features.Documents.Popups;

public partial class DocumentsPopup : Popup
{
    public DocumentsViewModel ViewModel { get; }

    public DocumentsPopup(int projectId, ApiService api)
    {
        InitializeComponent();

        ViewModel = new DocumentsViewModel(api);
        BindingContext = ViewModel;

        _ = ViewModel.Load(projectId);
    }

    private void Close(object sender, EventArgs e)
    {
        CloseAsync();
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;

#if WINDOWS
    if (e.PlatformArgs?.DragEventArgs != null)
        e.PlatformArgs.DragEventArgs.AcceptedOperation =
            Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
#endif

        DropZone.BackgroundColor = Color.FromArgb("#2D3F55");
        DropLabel.Text = "Release to upload";
    }

    private void OnDragLeave(object sender, DragEventArgs e)
    {
        DropZone.BackgroundColor = Color.FromArgb("#1F2937");
        DropLabel.Text = "Drag and Drop or pick file";
    }

    private async void OnDrop(object sender, DropEventArgs e)
    {
        DropZone.BackgroundColor = Color.FromArgb("#1F2937");
        DropLabel.Text = "Drag and Drop or pick file";

#if WINDOWS
    try
    {
        var windowsData = e.PlatformArgs?.DragEventArgs?.DataView;
        if (windowsData == null) return;

        if (windowsData.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
        {
            var items = await windowsData.GetStorageItemsAsync();
            foreach (var item in items)
            {
                if (item is Windows.Storage.StorageFile file)
                    await ViewModel.UploadFileAsync(file.Path, file.Name);
            }
        }
    }
    catch { }
#endif
    }
}