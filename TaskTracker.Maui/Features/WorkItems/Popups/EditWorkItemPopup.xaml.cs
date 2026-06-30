using CommunityToolkit.Maui.Views;
using TaskTracker.Maui.Common.Responses;

namespace TaskTracker.Maui.Popups;

public partial class EditWorkItemPopup : Popup
{
    private readonly TaskCompletionSource<EditWorkItemResult?> _tcs =
        new();

    public Task<EditWorkItemResult?> Result => _tcs.Task;

    public EditWorkItemPopup(string title, string comment)
    {
        InitializeComponent();

        TitleEntry.Text = title;
        CommentEditor.Text = comment;
    }

    private async void SaveClicked(object sender, EventArgs e)
    {
        var result = new EditWorkItemResult
        {
            Title = TitleEntry.Text ?? "",
            Comment = CommentEditor.Text ?? ""
        };

        _tcs.TrySetResult(result);

        await CloseAsync();
    }

    private async void CancelClicked(object sender, EventArgs e)
    {
        _tcs.TrySetResult(null);

        await CloseAsync();
    }
    public void ApplyFieldErrors(List<ApiError> errors)
    {
        TitleErrorLabel.Text = "";
        CommentErrorLabel.Text = "";

        foreach (var error in errors)
        {
            switch (error.Field)
            {
                case "Title":
                    TitleErrorLabel.Text = error.Error;
                    break;

                case "Comment":
                    CommentErrorLabel.Text = error.Error;
                    break;
            }
        }
    }
}

public class EditWorkItemResult
{
    public string Title { get; set; } = "";
    public string Comment { get; set; } = "";
}