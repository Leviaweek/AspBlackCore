namespace WebServer.Interfaces;

public interface IHttpRequest
{
    public string Method { get; }
    public string Path { get; }
    public Dictionary<string, string> Queries { get; }
    public Dictionary<string, string> Headers { get; }

    public bool HasBody { get; }

    public T GetBody<T>();
    public object GetBody(Type type);
    
}