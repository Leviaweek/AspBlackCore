using AspBlackCore.Enums;
using AspBlackCore.Extensions;

namespace AspBlackCore.Interfaces;

internal abstract class HttpResponseBuilderBase: IHttpResponseBuilder, IWriteable
{
    private static readonly byte[] HttpVersion = "HTTP/1.1"u8.ToArray();
    private readonly Dictionary<string, string> _headers = new();
    protected byte[] Body = [];

    private HttpStatusCode _statusCode = HttpStatusCode.Ok;

    public IHttpResponseBuilder AddHeader(string key, string value)
    {
        _headers[key] = value;
        return this;
    }
    public IHttpResponseBuilder SetHeaders(Dictionary<string, string> headers)
    {
        foreach (var (key, value) in headers)
        {
            AddHeader(key, value);
        }
        return this;
    }

    public abstract IHttpResponseBuilder SetBody<T>(T body);

    public IHttpResponseBuilder SetStatusCode(int statusCode) => SetStatusCode((HttpStatusCode) statusCode);

    protected IHttpResponseBuilder SetStatusCode(HttpStatusCode statusCode)
    {
        _statusCode = statusCode;
        return this;
    }

    public async Task WriteAsync(Stream stream, CancellationToken cancellationToken)
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
        
        WriteAsciiString(stream, statusDescription);
        
        AddEndLine(stream);
        
        foreach (var (key, value) in _headers)
        {
            WriteAsciiString(stream, key);
            stream.WriteByte((byte)':');
            stream.WriteByte((byte)' ');
            WriteAsciiString(stream, value);
            AddEndLine(stream);
        }
        
        AddEndLine(stream);
        
        if (Body.Length > 0)
        {
            await stream.WriteAsync(Body, cancellationToken);
        }
    }

    protected static void WriteAsciiString(Stream stream, string @string)
    {
        for (var index = 0; index < @string.Length; index++)
        {
            var character = @string[index];
            stream.WriteByte((byte)character);
        }
    }

    protected static void AddEndLine(Stream stream)
    {
        stream.WriteByte((byte)'\r');
        stream.WriteByte((byte)'\n');
    }
}