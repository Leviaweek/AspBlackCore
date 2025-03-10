namespace BlackDependencyInjection.Interfaces;

public interface IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken);
    public Task StopAsync(CancellationToken cancellationToken);
}