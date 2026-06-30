using System.Net.Http.Headers;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui.Infrastructure;

public class AuthHandler : DelegatingHandler
{
    private readonly TokenStore _tokenStore;

    public AuthHandler(TokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = _tokenStore.Get();

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}