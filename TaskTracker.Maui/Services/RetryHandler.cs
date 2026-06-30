using System;
using System.Collections.Generic;
namespace TaskTracker.Maui.Services;

public class RetryHandler : DelegatingHandler
{
    private const int MaxAttempts = 3;

    protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
    {
        if (request.Content is MultipartFormDataContent or StreamContent)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            var requestClone = await CloneAsync(request);

            try
            {
                return await base.SendAsync(requestClone, cancellationToken);
            }
            catch (IOException) when (attempt < MaxAttempts)
            {
                await Task.Delay(150 * attempt, cancellationToken);
            }
            catch (HttpRequestException) when (attempt < MaxAttempts)
            {
                await Task.Delay(150 * attempt, cancellationToken);
            }
        }

        var finalRequest = await CloneAsync(request);
        return await base.SendAsync(finalRequest, cancellationToken);
    }

    private static async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version
        };

        foreach (var header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        foreach (var option in request.Options)
            clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);

        if (request.Content != null)
        {
            var contentBytes = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(contentBytes);

            foreach (var header in request.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}