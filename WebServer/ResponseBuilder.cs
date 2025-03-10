using System.Text;
using WebServer.Enums;
using WebServer.Extensions;

namespace WebServer;

public sealed class ResponseBuilder
{
    private static readonly byte[] HttpVersion = "HTTP/1.1"u8.ToArray();
    private readonly Dictionary<string, string> _headers = new();
    private string _body = string.Empty;

    private HttpStatusCode _statusCode = HttpStatusCode.Ok;

    public ResponseBuilder AddHeader(string key, string value)
    {
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

    public ResponseBuilder SetJsonBody(string body)
    {
        AddHeader("Content-Type", "application/json");
        AddHeader("Content-Length", Encoding.UTF8.GetByteCount(body).ToString());
        _body = body;
        return this;
    }
    
    public async Task WriteAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        await stream.WriteAsync(HttpVersion, cancellationToken);
        stream.WriteByte((byte)' ');

        var statusCode = (int) _statusCode;
        var firstChar = (byte)(statusCode / 100 + '0');
        var secondChar = (byte)(statusCode / 10 % 10 + '0');
        var thirdChar = (byte)(statusCode % 10 + '0');
        
        stream.WriteByte(firstChar);
        stream.WriteByte(secondChar);
        stream.WriteByte(thirdChar);
        
        stream.WriteByte((byte)' ');
        
        var statusDescription = _statusCode.GetDescription();
        
        WriteString(stream, statusDescription);
        
        AddEndLine(stream);
        
        foreach (var (key, value) in _headers)
        {
            WriteString(stream, key);
            stream.WriteByte((byte)':');
            stream.WriteByte((byte)' ');
            WriteString(stream, value);
            AddEndLine(stream);
        }
        
        AddEndLine(stream);
        
        if (!string.IsNullOrEmpty(_body))
        {
            WriteString(stream, _body);
        }
    }

    private static void WriteString(Stream stream, string @string)
    {
        for (var index = 0; index < @string.Length; index++)
        {
            var character = @string[index];
            stream.WriteByte((byte)character);
        }
    }

    private static void AddEndLine(Stream stream)
    {
        stream.WriteByte((byte)'\r');
        stream.WriteByte((byte)'\n');
    }
}