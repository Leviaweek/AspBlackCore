using System.Collections;
using System.Diagnostics.CodeAnalysis;
using BlackDependencyInjection.Base;
using BlackDependencyInjection.Interfaces;

namespace BlackDependencyInjection;

public sealed class BlackServiceCollection: IBlackServiceCollection
{
    internal readonly Dictionary<Type, ServiceDescriptor> ServiceDescriptors = new();
    
    private void AddDescriptor<TService, TImplementation>(ServiceLifetime lifetime, Func<IBlackServiceProvider, object>? factory = null)
        where TService : class where TImplementation : class, TService =>
        ServiceDescriptors[typeof(TService)] = new ServiceDescriptor(typeof(TService),
            typeof(TImplementation),
            lifetime,
            factory);

    public void AddSingleton<TService>(TService service) where TService : class =>
        AddDescriptor<TService, TService>(ServiceLifetime.Singleton, _ => service);

    public void AddSingleton<TService>(Func<IBlackServiceProvider, TService>? factory = null) where TService : class => 
        AddDescriptor<TService, TService>(ServiceLifetime.Singleton, factory);
    
    public void AddSingleton<TService, TImplementation>(Func<IBlackServiceProvider, TService>? factory = null)
        where TService : class
        where TImplementation : class, TService =>
        AddDescriptor<TService, TImplementation>(ServiceLifetime.Singleton, factory);

    public void AddSingleton(Type serviceType, Type implementationType, Func<IBlackServiceProvider, object>? factory = null) =>
        ServiceDescriptors[serviceType] =
            new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Singleton, factory);

    public void AddSingleton(Type serviceType, Func<IBlackServiceProvider, object>? factory = null) =>
        AddSingleton(serviceType, serviceType, factory);
    
    public void AddScoped<TService, TImplementation>(Func<IBlackServiceProvider, TService>? factory = null)
        where TService : class where TImplementation : class, TService =>
        AddDescriptor<TService, TImplementation>(ServiceLifetime.Scoped, factory);
    

    public void AddScoped<TService>(Func<IBlackServiceProvider, TService>? factory = null) where TService : class =>
        AddDescriptor<TService, TService>(ServiceLifetime.Scoped, factory);

    public void AddScoped(Type serviceType, Type implementationType, Func<IBlackServiceProvider, object>? factory = null) =>
        ServiceDescriptors[serviceType] =
            new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Scoped, factory);

    public void AddScoped(Type serviceType, Func<IBlackServiceProvider, object>? factory = null) =>
        AddScoped(serviceType, serviceType, factory);
    

    public void AddTransient<TService>(Func<IBlackServiceProvider, TService>? factory = null) where TService : class =>
        AddDescriptor<TService, TService>(ServiceLifetime.Transient, factory);

    public void AddTransient<TService, TImplementation>(Func<IBlackServiceProvider, TService>? factory = null) where TService : class
        where TImplementation : class, TService =>
        AddDescriptor<TService, TImplementation>(ServiceLifetime.Transient, factory);

    public void AddTransient(Type serviceType, Type implementationType, Func<IBlackServiceProvider, object>? factory = null) =>
        ServiceDescriptors[serviceType] =
            new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Transient, factory);

    public void AddTransient(Type serviceType, Func<IBlackServiceProvider, object>? factory = null) =>
        AddTransient(serviceType, serviceType, factory);

    public IEnumerator<KeyValuePair<Type, ServiceDescriptor>> GetEnumerator() => ServiceDescriptors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(KeyValuePair<Type, ServiceDescriptor> item) => ServiceDescriptors.Add(item.Key, item.Value);
    public void Clear() => ServiceDescriptors.Clear();

    public bool Contains(KeyValuePair<Type, ServiceDescriptor> item) => ServiceDescriptors.Contains(item);

    public void CopyTo(KeyValuePair<Type, ServiceDescriptor>[] array, int arrayIndex) =>
        ((ICollection)ServiceDescriptors).CopyTo(array, arrayIndex);
    
    public bool Remove(KeyValuePair<Type, ServiceDescriptor> item) => ServiceDescriptors.Remove(item.Key);

    public int Count => ServiceDescriptors.Count;
    public bool IsReadOnly => false;
    public void Add(Type key, ServiceDescriptor value) => ServiceDescriptors.Add(key, value);

    public bool ContainsKey(Type key) => ServiceDescriptors.ContainsKey(key);

    public bool Remove(Type key) => ServiceDescriptors.Remove(key);

    public bool TryGetValue(Type key, [MaybeNullWhen(false)] out ServiceDescriptor value) =>
        ServiceDescriptors.TryGetValue(key, out value);
    
    public ServiceDescriptor this[Type key]
    {
        get => ServiceDescriptors[key];
        set => ServiceDescriptors[key] = value;
    }

    public ICollection<Type> Keys => ServiceDescriptors.Keys;
    public ICollection<ServiceDescriptor> Values => ServiceDescriptors.Values;
}