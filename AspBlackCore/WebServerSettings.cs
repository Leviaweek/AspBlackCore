using System.Text.Json.Serialization;

namespace AspBlackCore;

[Serializable]
public sealed record WebServerSettings(
    [property: JsonPropertyName("ipAddress")]string IpAddress,
    [property: JsonPropertyName("port")]int Port);