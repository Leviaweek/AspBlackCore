namespace WebServer.Interfaces;

public interface IBlackServer : IDisposable
{
    public Task StartAsync(CancellationToken cancellationToken);
}