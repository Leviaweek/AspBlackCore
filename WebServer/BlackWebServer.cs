using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using WebServer.Exceptions;
using WebServer.Interfaces;
using WebServer.Models;

namespace WebServer;

public sealed class BlackWebServer: IBlackServer
{
    private readonly TcpListener _listener;
    private readonly ChannelWriter<BlackWebServerRequestCell> _writer;
    
    private readonly HttpRequestParser _requestParser;
    
    public BlackWebServer(IPAddress ip, int port, ChannelWriter<BlackWebServerRequestCell> writer,
        uint maxBodySize = 1024 * 1024 * 25)
    {
        _writer = writer;
        _requestParser = new HttpRequestParser(maxBodySize);
        _listener = new TcpListener(ip, port);
    }

    public BlackWebServer(ChannelWriter<BlackWebServerRequestCell> writer): this(IPAddress.Loopback, 8080, writer) { }

    public BlackWebServer(string ip, int port, ChannelWriter<BlackWebServerRequestCell> writer) : this(
        IPAddress.Parse(ip), port, writer)
    { }
    public BlackWebServer(int port, ChannelWriter<BlackWebServerRequestCell> writer) : this(IPAddress.Loopback, port,
        writer)
    { }

    
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
        var stream = client.GetStream();
        try
        {
            var request = await _requestParser.ParseRequestAsync(stream, cancellationToken);
            var cell = new BlackWebServerRequestCell(request, client);
            await _writer.WriteAsync(cell, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Request processing canceled");
            client.Dispose();
        }
        catch (ParseRequestException ex)
        {
            Console.WriteLine($"Error parsing request: {ex.Message}");
            client.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            client.Dispose();
        }
    }

    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _listener.Dispose();
    }
    
    ~BlackWebServer()
    {
        Dispose();
    }
}

public sealed record BlackWebServerRequestCell(JsonRequest Request, TcpClient Client);