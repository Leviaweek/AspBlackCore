using BlackDependencyInjection.Interfaces;

namespace BlackDependencyInjection;

public sealed class ScopedBlackServiceProvider: BlackServiceProviderBase, IScopedBlackServiceProvider
{
    internal readonly Dictionary<Type, object> ScopedServices = new();
    
    internal IBlackServiceProvider RootBlackServiceProvider { get; set; }

    public ScopedBlackServiceProvider(IBlackServiceCollection blackServiceCollection, IBlackServiceProvider rootBlackServiceProvider) : base(blackServiceCollection)
    {
        RootBlackServiceProvider = rootBlackServiceProvider;
    }

    public override object? GetService(Type serviceType)
    {
        if (!BlackServiceCollection.TryGetValue(serviceType, out var serviceDescriptor)) return null;
        
        switch (serviceDescriptor.Lifetime)
        {
            case ServiceLifetime.Scoped when ScopedServices.TryGetValue(serviceType, out var service):
            {
                return service;
            }
            case ServiceLifetime.Scoped:
            {
                var result = CreateService(serviceDescriptor);
                if (result is null) return result;
                ScopedServices[serviceType] = result;
                return result;
            }
            case ServiceLifetime.Singleton:
                return RootBlackServiceProvider.GetService(serviceType);
            case ServiceLifetime.Transient:
                return CreateService(serviceDescriptor);
            default:
                throw new InvalidOperationException("Lifetime not found");
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (var service in ScopedServices.Values)
        {
            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        ScopedServices.Clear();
    }
    
    ~ScopedBlackServiceProvider()
    {
        Dispose();
    }
}