using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using BlackDependencyInjection.Interfaces;
using WebServer.Exceptions;
using WebServer.Models;

namespace WebServer;

public sealed class WebServer: IDisposable
{
    private readonly TcpListener _listener;
    private readonly HttpRequestParser _requestParser;

    public readonly Dictionary<MethodEndpoint, Delegate> Maps = new();
    internal readonly IBlackServiceProvider ServiceProvider;
    
    public WebServer(IPAddress ip, int port, IBlackServiceProvider serviceProvider, int maxBodySize = 1024 * 1024 * 25)
    {
        ServiceProvider = serviceProvider;
        _requestParser = new HttpRequestParser(maxBodySize);
        _listener = new TcpListener(ip, port);
    }

    public WebServer(IBlackServiceProvider serviceProvider): this(IPAddress.Loopback, 8080, serviceProvider) { }

    public WebServer(string ip, int port, IBlackServiceProvider serviceProvider) : this(IPAddress.Parse(ip), port,
        serviceProvider) { }
    
    public WebServer(int port, IBlackServiceProvider serviceProvider): this(IPAddress.Loopback, port, serviceProvider) {}
    
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _listener.Start();

        while (!cancellationToken.IsCancellationRequested)
        {
            var client = await _listener.AcceptTcpClientAsync(cancellationToken);
            _ = Task.Run(() => ProcessClientAsync(client, cancellationToken), cancellationToken)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine(task.Exception);
                    }
                }, cancellationToken);
        }
    }

    private async Task ProcessClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        await using var stream = client.GetStream();
        try
        {
            var request = await _requestParser.ParseRequestAsync(stream, cancellationToken);

            var response = CreateResponse(request);
            
            await response.WriteAsync(stream, cancellationToken);
        }
        catch (BodyTooLargeException)
        {
            await new ResponseBuilder()
                .SetStatusCode(413)
                .WriteAsync(stream, cancellationToken);
        }
        catch (BadContentLengthException)
        {
            await new ResponseBuilder()
                .SetStatusCode(400)
                .WriteAsync(stream, cancellationToken);
        }
        finally
        {
            client.Dispose();
        }
    }

    private ResponseBuilder CreateResponse(Request request)
    {
        var methodEndpoint = new MethodEndpoint(request.Method, request.Path);
        if (!Maps.TryGetValue(methodEndpoint, out var handler))
        {
            return new ResponseBuilder().SetStatusCode(404);
        }
            
        var parameters = handler.Method.GetParameters();

        var args = new object?[parameters.Length];

        if (!InsertParameters(parameters, request, args)) return new ResponseBuilder().SetStatusCode(400);
        
        var responseType = handler.Method.ReturnType;

        var responseObject = handler.Method.Invoke(handler.Target, args);

        var response = new ResponseBuilder().SetStatusCode(200);
        
        if (responseType != typeof(void)) response.SetJsonBody(JsonSerializer.SerializeToUtf8Bytes(responseObject));

        return response;
    }

    private bool InsertParameters(ParameterInfo[] parameters, Request request, object?[] args)
    {
        var isBodyUsed = false;
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            var name = parameter.Name;
                
            if (name is not null)
            {
                if (request.Queries.TryGetValue(name, out var value) && parameter.ParameterType == typeof(string))
                {
                    args[i] = value;
                    continue;
                }
            }
            var service = ServiceProvider.GetService(parameter.ParameterType);
            if (service is null)
            {
                if (isBodyUsed) return false;
                args[i] = JsonSerializer.Deserialize(request.Body, parameter.ParameterType);
                isBodyUsed = true;
                continue;
            }
            args[i] = service;
            
        }

        return true;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _listener.Dispose();
    }
    
    ~WebServer() => Dispose();
}