using System.Runtime.InteropServices;
using System.Threading.Channels;
using AspBlackCore.Builders;
using AspBlackCore.Delegates;
using AspBlackCore.Dispatchers;
using AspBlackCore.Interfaces;
using AspBlackCore.Middlewares;
using AspBlackCore.Models;
using BlackDependencyInjection;
using BlackDependencyInjection.Interfaces;
using WebServer;
using WebServer.Exceptions;

namespace AspBlackCore;

public sealed class App
{
    private readonly RootBlackServiceProvider _serviceProvider;
    private readonly BlackWebServer _server;
    private readonly ChannelReader<BlackWebServerRequestCell> _reader;
    private readonly MinimalApiDispatcher _minimalApiDispatcher;
    private readonly MiddlewarePipelineBuilder _pipelineBuilder;

    internal App(RootBlackServiceProvider serviceProvider, BlackWebServer server,
        ChannelReader<BlackWebServerRequestCell> reader)
    {
        _serviceProvider = serviceProvider;
        _server = server;
        _reader = reader;
        _minimalApiDispatcher = serviceProvider.GetRequiredService<MinimalApiDispatcher>();
        _pipelineBuilder = serviceProvider.GetRequiredService<MiddlewarePipelineBuilder>();
    }
    
    public void MapGet(string path, Delegate handler) => MapMinimalApi(HttpMethods.Get, path, handler);
    public void MapPost(string path, Delegate handler) => MapMinimalApi(HttpMethods.Post, path, handler);
    public void MapPut(string path, Delegate handler) => MapMinimalApi(HttpMethods.Put, path, handler);
    public void MapDelete(string path, Delegate handler) => MapMinimalApi(HttpMethods.Delete, path, handler);
    
    private void MapMinimalApi(string method, string path, Delegate handler) =>
        _minimalApiDispatcher.Map(method, path, handler);
    
    public void UseMiddleware<TMiddleware>() where TMiddleware : Middleware => _pipelineBuilder.Use<TMiddleware>();
    public void UseRouting()
    {
        _pipelineBuilder.Use<ControllerMiddleware>();
        _pipelineBuilder.Use<MinimalApiMiddleware>();
    }

    public async Task StartAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        using var tokenSource = new CancellationTokenSource();

        var appContext = new BlackAppContext(tokenSource);
        
        var delegatePipeline = _pipelineBuilder.Build();
        
        _serviceProvider.GetRequiredService<IBlackServiceCollection>().AddSingleton(appContext);
        
        using var signal = PosixSignalRegistration.Create(PosixSignal.SIGINT, context =>
        {
            // ReSharper disable once AccessToDisposedClosure
            tokenSource.Cancel();
            context.Cancel = true;
        });
        try
        {
            var webServerTask = _server.StartAsync(tokenSource.Token).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine(task.Exception);
                }
            }, tokenSource.Token);

            await foreach (var cell in _reader.ReadAllAsync(tokenSource.Token))
            {
                // ReSharper disable once AccessToDisposedClosure
                _ = Task.Run(() => ProcessCellAsync(cell, delegatePipeline, tokenSource.Token), tokenSource.Token)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Console.WriteLine(task.Exception);
                        }
                    }, tokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Server stopped");
        }
        finally
        {
            _serviceProvider.Dispose();
        }
    }
    
    private async Task ProcessCellAsync(BlackWebServerRequestCell cell, RequestDelegate @delegate,
        CancellationToken cancellationToken)
    {
        await using var stream = cell.Client.GetStream();
        try
        {
            var request = cell.Request;
            
            var response = new HttpResponseBuilder();
            
            var context = new HttpContext(request, response);
            await @delegate.Invoke(context);
            
            if (context.Response is IWriteable writeable)
                await writeable.WriteAsync(stream, cancellationToken);
            else
            {
                var builder = new HttpResponseBuilder();
                builder.SetStatusCode(500);
                await builder.WriteAsync(stream, cancellationToken);
            }
        }
        catch (BodyTooLargeException)
        {
            var builder = new HttpResponseBuilder();
            builder.SetStatusCode(413);
            await builder.WriteAsync(stream, cancellationToken);
        }
        catch (BadContentLengthException)
        {
            var builder = new HttpResponseBuilder();
            builder.SetStatusCode(400);
            await builder.WriteAsync(stream, cancellationToken);
        }
        finally
        {
            stream.Close();
            cell.Client.Dispose();
        }
    }
}