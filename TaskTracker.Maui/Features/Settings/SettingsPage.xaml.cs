using TaskTracker.Maui.Infrastructure;

namespace TaskTracker.Maui.Features.Settings;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        ApiUrlEntry.Text = Preferences.Default.Get("ApiBaseUrl", "http://localhost:8080/");
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ApiUrlEntry.Text))
            return;

        AppConfig.SaveApiBaseUrl(ApiUrlEntry.Text);

        await DisplayAlertAsync("Сохранено", "Перезапустите приложение, чтобы изменения вступили в силу.", "OK");
    }
}