using System.Text;
using System.Text.Json;
using WebServer.Enums;
using WebServer.Extensions;

namespace WebServer;

public sealed class ResponseBuilder
{
    private const string HeaderTemplate = "HTTP/1.1 {0} {1}";
    private readonly Dictionary<string, string> _headers = new();
    private byte[] _body = [];
    private int _responseSize = 0;

    private HttpStatusCode _statusCode = HttpStatusCode.Ok;

    public ResponseBuilder AddHeader(string key, string value)
    {
        _responseSize += key.Length + value.Length + 4;
        _headers[key] = value;
        return this;
    }
    public ResponseBuilder SetHeaders(Dictionary<string, string> headers)
    {
        foreach (var (key, value) in headers)
        {
            AddHeader(key, value);
        }
        return this;
    }

    public ResponseBuilder SetStatusCode(int statusCode) => SetStatusCode((HttpStatusCode) statusCode);

    public ResponseBuilder SetStatusCode(HttpStatusCode statusCode)
    {
        _statusCode = statusCode;
        return this;
    }

    public ResponseBuilder SetJsonBody(byte[] body)
    {
        AddHeader("Content-Type", "application/json");
        AddHeader("Content-Length", body.Length.ToString());
        _body = body;
        return this;
    }

    public byte[] Build()
    {
        var response = new StringBuilder();
        response.AppendLine(string.Format(HeaderTemplate, (int)_statusCode, _statusCode.GetDescription()));
        
        foreach (var (key, value) in _headers)
        {
            response.AppendLine($"{key}: {value}");
        }
        
        response.AppendLine();
        if (_body.Length > 0)
        {
            response.AppendLine(Encoding.UTF8.GetString(_body));
        }
        
        return Encoding.UTF8.GetBytes(response.ToString());
    }
}