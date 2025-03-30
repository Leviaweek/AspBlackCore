using System.Reflection;
using WebServer.Attributes;
using WebServer.Exceptions;
using WebServer.Extensions;
using WebServer.Models;

namespace WebServer;

public sealed class HttpRequestParser
{
    internal readonly int MaxBodySize;

    private delegate Task<byte[]> ParseBodyAsyncDelegate(Stream stream,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken);

    private readonly Dictionary<string, ParseBodyAsyncDelegate> _contentTypeHandlers = new(StringComparer.OrdinalIgnoreCase);

    public HttpRequestParser(int maxBodySize)
    {
        MaxBodySize = maxBodySize;
        RegisterHandlers();
    }

    private void RegisterContentTypeHandler(string contentType, ParseBodyAsyncDelegate handler) =>
        _contentTypeHandlers[contentType] = handler;

    private void RegisterHandlers()
    {
        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        
        foreach (var method in methods)
        {
            var attributes = method.GetCustomAttributes<ContentTypeHandlerAttribute>();
            
            foreach (var attribute in attributes)
            {
                var handler = (ParseBodyAsyncDelegate)method.CreateDelegate(typeof(ParseBodyAsyncDelegate), this);
                RegisterContentTypeHandler(attribute.ContentType, handler);
            }
        }
    }
    
    public async Task<Request> ParseRequestAsync(Stream stream, CancellationToken cancellationToken)
    {
            using var src = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(src.Token, cancellationToken);
            
            var token = linked.Token;

            var firstLine = await stream.ReadLineAsync(cancellationToken: token);

            firstLine = ValidateLine(firstLine);

            if (firstLine.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries) is not { Length: 3 } parts)
                throw new ParseRequestException("Invalid request line");
            
            var (method, fullPath, version) = (parts[0], parts[1], parts[2]);
            
            var pathSegments = fullPath.Split('?');
            
            if (pathSegments.Length > 2) throw new ParseRequestException("Invalid path");
            
            var path = pathSegments[0];
            
            var queries = ParseQueries(pathSegments);

            var headers = await ParseHeadersAsync(stream, token);

            byte[] bytes;

            if (headers.TryGetValue("content-type", out var contentType))
            {
                if (!_contentTypeHandlers.TryGetValue(contentType, out var handler))
                {
                    throw new ParseRequestException("Unsupported Content-Type");
                }

                bytes = await handler(stream, headers, token);
            }
            else
            {
                bytes = [];
            }
            
            return new Request(method, path, version, queries, headers, bytes);
    }

    private static async Task<Dictionary<string, string>> ParseHeadersAsync(Stream stream, CancellationToken cancellationToken)
    {
        string? line;
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
        while (!string.IsNullOrWhiteSpace(line = await stream.ReadLineAsync(cancellationToken: cancellationToken)))
        {
            if (line.Split(": ", 2) is not { Length: 2 } header) throw new ParseRequestException("Invalid header");

            headers[header[0]] = header[1];
        }

        return headers;
    }

    private static Dictionary<string, string> ParseQueries(string[] pathSegments)
    {
        var queries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (pathSegments.Length != 2) return queries;
        
        foreach (var query in pathSegments[1].Split('&'))
        {
            if (query.Split('=', 2) is not { Length: 2 } queryParts) throw new ParseRequestException("Invalid query");
            queries[queryParts[0]] = queryParts[1];
        }
        return queries;
    }

    private static string ValidateLine(string? line)
    {
        return line switch
        {
            null => throw new ParseRequestException("Unexpected end of stream"),
            "" => throw new ParseRequestException("Empty request line"),
            _ => line
        };
    }
    
    [ContentTypeHandler("application/json")]
    public async Task<byte[]> ParseJsonBodyAsync(Stream stream,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken)
    {

        if (!headers.TryGetValue("content-length", out var contentLengthStr))
            throw new ParseRequestException("Missing Content-Length header");
    
        var contentLength = int.Parse(contentLengthStr);
        
        if (contentLength > MaxBodySize) throw new BodyTooLargeException();
        
        var bytes = new byte[contentLength];
        
        await stream.ReadExactlyAsync(bytes, cancellationToken);

        return bytes;
    }
}