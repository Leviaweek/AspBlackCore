using BlackDependencyInjection.Interfaces;

namespace BlackDependencyInjection;

public sealed class BlackServiceScope : IBlackServiceScope
{
    public IBlackServiceProvider ServiceProvider { get; }

    public BlackServiceScope(IBlackServiceProvider blackServiceProvider)
    {
        ServiceProvider = blackServiceProvider;
    }

    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}