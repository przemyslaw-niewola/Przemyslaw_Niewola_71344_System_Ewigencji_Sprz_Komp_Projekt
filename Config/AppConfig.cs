using System.Text.Json;

namespace EwidencjaSprzetuOOP.Config;

public sealed class AppConfig
{
    public ConnectionStrings ConnectionStrings { get; set; } = new();

    public static AppConfig Load(string path = "appsettings.json")
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Brak pliku konfiguracyjnego: {path}");

        var json = File.ReadAllText(path);
        var cfg = JsonSerializer.Deserialize<AppConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (cfg is null || string.IsNullOrWhiteSpace(cfg.ConnectionStrings.Default))
            throw new InvalidOperationException("Nieprawidłowy appsettings.json lub brak ConnectionStrings:Default.");

        return cfg;
    }
}

public sealed class ConnectionStrings
{
    public string Default { get; set; } = "";
}
