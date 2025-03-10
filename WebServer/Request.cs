namespace WebServer;

public sealed record Request(
    string Method,
    string Path,
    string Version,
    Dictionary<string, string> Queries,
    Dictionary<string, string> Headers,
    string Body);