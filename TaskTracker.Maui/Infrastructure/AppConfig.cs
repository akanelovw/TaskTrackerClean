using System.Reflection;
using System.Text.Json;

namespace TaskTracker.Maui.Infrastructure;

public class AppConfig
{
    private const string ConfigFileName = "config.json";
    private const string DefaultUrl = "http://localhost:8080/";
    private const string PreferencesKey = "ApiBaseUrl";

    public string ApiBaseUrl { get; private set; } = DefaultUrl;

#if WINDOWS || MACCATALYST
    // Для PublishSingleFile=true сборок AppContext.BaseDirectory может
    // указывать на временную extraction-папку, а не на реальное
    // расположение .exe. Process.MainModule.FileName даёт настоящий путь
    // к запущенному исполняемому файлу независимо от способа публикации.
    private static string GetExecutableDirectory()
    {
        try
        {
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
            if (!string.IsNullOrEmpty(exePath))
                return Path.GetDirectoryName(exePath) ?? AppContext.BaseDirectory;
        }
        catch
        {
            // fallback ниже
        }
        return AppContext.BaseDirectory;
    }
#endif

    public static AppConfig Load()
    {
        var config = new AppConfig();

        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("TaskTracker.Maui.appsettings.json");

            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();

                var parsed = JsonSerializer.Deserialize<ConfigFile>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (!string.IsNullOrWhiteSpace(parsed?.ApiBaseUrl))
                    config.ApiBaseUrl = Normalize(parsed.ApiBaseUrl);
            }
        }
        catch
        {
            // embedded resource не найден — используем дефолт
        }

#if WINDOWS || MACCATALYST
        try
        {
            var configPath = Path.Combine(GetExecutableDirectory(), ConfigFileName);

            if (!File.Exists(configPath))
            {
                var content = JsonSerializer.Serialize(
                    new { ApiBaseUrl = config.ApiBaseUrl },
                    new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(configPath, content);
            }
            else
            {
                var json = File.ReadAllText(configPath);
                var parsed = JsonSerializer.Deserialize<ConfigFile>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (!string.IsNullOrWhiteSpace(parsed?.ApiBaseUrl))
                    config.ApiBaseUrl = Normalize(parsed.ApiBaseUrl);
            }
        }
        catch
        {
            // config.json повреждён или нет доступа — оставляем уже загруженное значение
        }
#endif

        return config;
    }

    public static void SaveApiBaseUrl(string url)
    {
        var normalized = Normalize(url);

#if WINDOWS || MACCATALYST
        try
        {
            var configPath = Path.Combine(GetExecutableDirectory(), ConfigFileName);
            var content = JsonSerializer.Serialize(
                new { ApiBaseUrl = normalized },
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(configPath, content);
        }
        catch
        {
            // нет прав на запись рядом с .exe — игнорируем
        }
#else
        Preferences.Default.Set(PreferencesKey, normalized);
#endif
    }

    public static string GetMobileOverrideUrl(string fallback)
    {
#if !WINDOWS && !MACCATALYST
        var saved = Preferences.Default.Get(PreferencesKey, string.Empty);
        if (!string.IsNullOrWhiteSpace(saved))
            return Normalize(saved);
#endif
        return fallback;
    }

    private static string Normalize(string url) =>
        url.TrimEnd('/') + "/";

    private class ConfigFile
    {
        public string? ApiBaseUrl { get; set; }
    }
}