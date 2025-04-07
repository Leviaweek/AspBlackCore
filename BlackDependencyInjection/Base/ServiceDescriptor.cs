using BlackDependencyInjection.Interfaces;

namespace BlackDependencyInjection.Base;

public sealed class ServiceDescriptor
{
    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public ServiceLifetime Lifetime { get; }
    public Func<IBlackServiceProvider, object>? Factory { get; }

    public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime,
        Func<IBlackServiceProvider, object>? factory = null)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Lifetime = lifetime;
        Factory = factory;
    }
}