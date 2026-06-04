using System.Text.Json;

namespace TaskTracker.Api.IntegrationTests.Common;

public static class JsonHelper
{
    public static int ExtractId(string json)
    {
        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        if (root.TryGetProperty("data", out var data))
        {
            if (data.ValueKind == JsonValueKind.Number)
                return data.GetInt32();

            if (data.ValueKind == JsonValueKind.Object &&
                data.TryGetProperty("id", out var idObj))
                return idObj.GetInt32();
        }

        if (root.TryGetProperty("id", out var id))
            return id.GetInt32();

        throw new Exception("ID not found");
    }
}