using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using BlackDependencyInjection.Interfaces;

namespace BlackDependencyInjection;

public abstract class BlackServiceProviderBase: IBlackServiceProvider
{
    internal readonly IBlackServiceCollection BlackServiceCollection;

    protected BlackServiceProviderBase(IBlackServiceCollection blackServiceCollection)
    {
        BlackServiceCollection = blackServiceCollection;
    }

    public T GetRequiredService<T>() where T : class
    {
        var type = typeof(T);
        var service = GetService(type) ?? throw new ServiceNotFoundException(type);
        return Unsafe.As<T>(service);
    }

    public abstract object? GetService(Type serviceType);

    public object? CreateService(ServiceDescriptor serviceDescriptor)
    {
        if (serviceDescriptor.Factory is not null) return serviceDescriptor.Factory(this);
        
        var constructor = serviceDescriptor.ImplementationType.GetConstructors().FirstOrDefault();
        
        if (constructor is null) return Activator.CreateInstance(serviceDescriptor.ImplementationType) 
                                        ?? throw new InvalidOperationException();

        var parameters = constructor.GetParameters();
        var resultParameters = new object[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            var service = GetService(parameters[i].ParameterType);
            if (service is null) return null;
            resultParameters[i] = service;
        }
        
        return Activator.CreateInstance(serviceDescriptor.ImplementationType, resultParameters) ??
                            throw new InvalidOperationException("Service dependency not found");
    }
    public IBlackServiceScope CreateScope() => new BlackServiceScope(new ScopedBlackServiceProvider(BlackServiceCollection, this));

}