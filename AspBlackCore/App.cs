using System.Runtime.InteropServices;
using BlackDependencyInjection;
using WebServer;
using WebServer.Models;

namespace AspBlackCore;

public sealed class App
{
    private readonly RootBlackServiceProvider _serviceProvider;
    private readonly WebServer.WebServer _server;

    public App(RootBlackServiceProvider serviceProvider, WebServer.WebServer server)
    {
        _serviceProvider = serviceProvider;
        _server = server;
    }
    
    public void MapGet(string path, Delegate handler) => Map(HttpMethods.Get, path, handler);
    public void MapPost(string path, Delegate handler) => Map(HttpMethods.Post, path, handler);
    public void MapPut(string path, Delegate handler) => Map(HttpMethods.Put, path, handler);
    public void MapDelete(string path, Delegate handler) => Map(HttpMethods.Delete, path, handler);
    
    private void Map(string method, string path, Delegate handler) =>
        _server.Maps.Add(new MethodEndpoint(method, path), handler);
    
    public async Task StartAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var tokenSource = scope.ServiceProvider.GetRequiredService<CancellationTokenSource>();
        
        using var signal = PosixSignalRegistration.Create(PosixSignal.SIGINT, context =>
        {
            tokenSource.Cancel();
            context.Cancel = true;
        });
        try
        {
            await _server.StartAsync(tokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Server stopped");
        }
    }
}