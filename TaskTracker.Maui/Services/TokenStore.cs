namespace TaskTracker.Maui.Services;

public class TokenStore
{
    private string? _token;

    public void Set(string token) => _token = token;

    public string? Get() => _token;
}