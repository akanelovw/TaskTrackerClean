using System.Net.Http.Json;
using System.Text.Json;
using TaskTracker.Maui.Common.Responses;

namespace TaskTracker.Maui.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly TokenStore _tokenStore;

    public AuthService(HttpClient http, TokenStore tokenStore)
    {
        _http = http;
        _tokenStore = tokenStore;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new
        {
            email,
            password
        });

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResult>>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (result == null)
            throw new Exception("Empty response from server");

        if (!response.IsSuccessStatusCode || !result.Success)
            throw new Exception(result.Message ?? "Login failed");

        var token = result.Data.Token;

        _tokenStore.Set(token);

        return token;
    }

    public async Task LogoutAsync()
    {
        _tokenStore.Set(string.Empty);
        await Task.CompletedTask;
    }
    public class LoginResult
    {
        public string Token { get; set; } = string.Empty;
    }
}