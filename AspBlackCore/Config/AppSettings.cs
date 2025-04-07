using System.Text.Json;

namespace AspBlackCore.Config;

public sealed class AppSettings
{
    public const string FileName = "applicationsettings.json";
    
    public required Dictionary<string, JsonElement> Settings { private get; init; }
    
    public WebServerSettings WebServerSettings => Get<WebServerSettings>("webServerSettings");
    public T Get<T>(string key) => Settings[key].Deserialize<T>() ?? throw new Exception("Invalid key not found");
}