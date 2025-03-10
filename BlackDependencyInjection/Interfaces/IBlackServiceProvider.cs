namespace BlackDependencyInjection.Interfaces;

public interface IBlackServiceProvider
{
    public T GetRequiredService<T>() where T : class;
    public object? GetService(Type serviceType);
    public IBlackServiceScope CreateScope();
}