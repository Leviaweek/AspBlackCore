namespace WebServer.Models;

public sealed record Request(
    string Method,
    string Path,
    string Version,
    Dictionary<string, string> Queries,
    Dictionary<string, string> Headers,
    byte[] Body);