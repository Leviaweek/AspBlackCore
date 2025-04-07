using System.Text.Json;
using WebServer.Exceptions;
using WebServer.Interfaces;

namespace WebServer.Models;

public sealed class JsonRequest : IHttpRequest
{
    public JsonRequest(string method,
        string path,
        Dictionary<string, string> queries,
        Dictionary<string, string> headers,
        byte[] body)
    {
        Method = method;
        Path = path;
        Queries = queries;
        
        Body = body;
        Headers = headers;
    }

    public string Method { get; }
    public string Path { get; }
    public Dictionary<string, string> Queries { get; }
    public Dictionary<string, string> Headers { get; }
    public bool HasBody => Body.Length > 0;
    public byte[] Body { get; }

    public T GetBody<T>()
    {
        try
        {
            var serialized = JsonSerializer.Deserialize<T>(Body);

            if (serialized is null) throw new ParseRequestException();

            return serialized;
        }
        catch
        {
            throw new ParseRequestException();
        }
    }
    
    public object GetBody(Type type)
    {
        try
        {
            var serialized = JsonSerializer.Deserialize(Body, type);

            if (serialized is null) throw new ParseRequestException();

            return serialized;
        }
        catch
        {
            throw new ParseRequestException();
        }
    }
}