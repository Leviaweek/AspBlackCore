using System.Runtime.CompilerServices;
using BlackDependencyInjection.Interfaces;

namespace BlackDependencyInjection;

public class RootBlackServiceProvider : BlackServiceProviderBase
{
    internal readonly Dictionary<Type, object> SingletonServices = new();
    public RootBlackServiceProvider(IBlackServiceCollection blackServiceCollection) : base(blackServiceCollection) { }
    public RootBlackServiceProvider() : base(new BlackServiceCollection()) { }

    public override object? GetService(Type serviceType)
    {
        if (!BlackServiceCollection.TryGetValue(serviceType, out var serviceDescriptor)) return null;
        
        switch (serviceDescriptor.Lifetime)
        {
            case ServiceLifetime.Scoped:
                throw new InvalidProviderException(this, serviceDescriptor);
            case ServiceLifetime.Singleton when SingletonServices.TryGetValue(serviceType, out var service):
                return service;
            case ServiceLifetime.Singleton:
            {
                var result = CreateService(serviceDescriptor);
                if (result is null) return null;
                SingletonServices[serviceType] = result;
                return result;
            }
            case ServiceLifetime.Transient:
                return CreateService(serviceDescriptor);
            default:
                throw new InvalidOperationException("Lifetime not found");
        }
    }
}

public abstract class DependencyInjectionException(string? message = null) : Exception(message);
public sealed class InvalidProviderException(
    IBlackServiceProvider provider,
    ServiceDescriptor descriptor)
    : DependencyInjectionException($"Provider {provider.GetType().FullName} is invalid for service {descriptor.ServiceType}");