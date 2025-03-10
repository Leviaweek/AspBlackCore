namespace BlackDependencyInjection.Interfaces;

public interface IBlackServiceScope : IDisposable
{
    IBlackServiceProvider ServiceProvider { get; }
}