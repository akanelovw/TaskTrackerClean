using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;

namespace TaskTracker.Maui.Features.Users.Popups;

public static class UserPickerPopupExtensions
{
    private static readonly PopupOptions DefaultOptions = new()
    {
        Shape = null,
        Shadow = null,
        PageOverlayColor = Colors.Black.WithAlpha(0.5f)
    };

    public static async Task<UserPickerItem?> ShowUserPickerAsync(
        string title,
        IEnumerable<UserPickerItem> users)
    {
        var popup = new UserPickerPopup(title, users);

        await Shell.Current.CurrentPage.ShowPopupAsync(popup, DefaultOptions);

        return await popup.Result;
    }
}