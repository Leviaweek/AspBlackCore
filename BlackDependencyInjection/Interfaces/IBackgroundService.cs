namespace BlackDependencyInjection.Interfaces;

public interface IBackgroundService
{
    public Task ExecuteAsync(CancellationToken cancellationToken);
}