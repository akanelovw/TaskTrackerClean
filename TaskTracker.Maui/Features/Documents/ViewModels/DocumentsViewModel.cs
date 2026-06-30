using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TaskTracker.Application.Documents.GetProjectDocuments;
using TaskTracker.Maui.Features.Base;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Features.Documents.ViewModels;

public partial class DocumentsViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private const int PageSize = 5;

    private List<GetProjectDocumentsResponse> _allDocuments = new();
    private int _projectId;

    public ObservableCollection<GetProjectDocumentsResponse> Documents { get; } = new();

    [ObservableProperty]
    private int page = 1;

    [ObservableProperty]
    private bool hasNextPage;

    [ObservableProperty]
    private bool hasPreviousPage;

    public DocumentsViewModel(ApiService api)
    {
        _api = api;
    }

    public async Task Load(int projectId)
    {
        _projectId = projectId;
        _allDocuments = await _api.GetProjectDocumentsAsync(projectId);

        Page = 1;
        ApplyPage();
    }

    private void ApplyPage()
    {
        var totalPages = Math.Max(1, (int)Math.Ceiling(_allDocuments.Count / (double)PageSize));

        if (Page > totalPages)
            Page = totalPages;

        Documents.Clear();

        foreach (var d in _allDocuments
            .Skip((Page - 1) * PageSize)
            .Take(PageSize))
        {
            Documents.Add(d);
        }

        HasNextPage = Page * PageSize < _allDocuments.Count;
        HasPreviousPage = Page > 1;
    }
    public async Task UploadFileAsync(string filePath, string fileName)
    {
        await using var stream = File.OpenRead(filePath);

        var result = await _api.UploadDocumentAsync(
            _projectId,
            fileName,
            stream);

        await HandleResult(result, async () =>
        {
            await Shell.Current.DisplayAlertAsync("Success", "File uploaded", "OK");
            await Load(_projectId);
        });
    }

    [RelayCommand]
    private void NextPage()
    {
        if (!HasNextPage)
            return;

        Page++;
        ApplyPage();
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (!HasPreviousPage)
            return;

        Page--;
        ApplyPage();
    }

    [RelayCommand]
    private async Task Upload()
    {
        var file = await FilePicker.PickAsync();
        if (file == null) return;

        await using var stream = await file.OpenReadAsync();

        var result = await _api.UploadDocumentAsync(
            _projectId,
            file.FileName,
            stream);

        await HandleResult(result, async () =>
        {
            await Shell.Current.DisplayAlertAsync(
                "Success",
                "File uploaded",
                "OK");

            await Load(_projectId);
        });
    }

    [RelayCommand]
    private async Task Delete(GetProjectDocumentsResponse doc)
    {
        if (doc == null)
            return;

        bool confirm =
            await Shell.Current.DisplayAlertAsync(
                "Delete file",
                $"Delete '{doc.FileName}'?",
                "Delete",
                "Cancel");

        if (!confirm)
            return;

        var result = await _api.DeleteDocumentAsync(_projectId, doc.Id);

        await HandleResult(result, async () =>
        {
            await Shell.Current.DisplayAlertAsync(
                "Success",
                "File deleted",
                "OK");

            await Load(_projectId);
        });
    }

    [RelayCommand]
    private async Task Open(GetProjectDocumentsResponse doc)
    {
        var action =
            await Shell.Current.DisplayActionSheetAsync(
                doc.FileName,
                "Cancel",
                null,
                "Open",
                "Download");

        if (action == "Cancel")
            return;

        var bytes =
            await _api.DownloadDocumentAsync(doc.Id);

        var tempPath = Path.Combine(
            FileSystem.CacheDirectory,
            doc.FileName);

        await File.WriteAllBytesAsync(
            tempPath,
            bytes);

        if (action == "Open")
        {
            await Launcher.OpenAsync(
                new OpenFileRequest
                {
                    File = new ReadOnlyFile(tempPath)
                });
        }

        if (action == "Download")
        {
            var result = await FolderPicker.Default.PickAsync();

            if (!result.IsSuccessful)
                return;

            var savePath = Path.Combine(
                result.Folder.Path,
                doc.FileName);

            await File.WriteAllBytesAsync(
                savePath,
                bytes);
        }
    }
}