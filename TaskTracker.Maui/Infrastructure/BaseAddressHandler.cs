namespace TaskTracker.Maui.Infrastructure;

public class BaseAddressHandler : DelegatingHandler
{
    private readonly string _fallbackUrl;

    public BaseAddressHandler(string fallbackUrl)
    {
        _fallbackUrl = fallbackUrl;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
#if !WINDOWS && !MACCATALYST
        var url = AppConfig.GetMobileOverrideUrl(_fallbackUrl);

        if (request.RequestUri != null && !string.IsNullOrWhiteSpace(url))
        {
            var baseUri = new Uri(url);
            var builder = new UriBuilder(request.RequestUri)
            {
                Scheme = baseUri.Scheme,
                Host = baseUri.Host,
                Port = baseUri.Port
            };
            request.RequestUri = builder.Uri;
        }
#endif
        return base.SendAsync(request, cancellationToken);
    }
}