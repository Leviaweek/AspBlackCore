namespace BlackDependencyInjection.Interfaces;

public interface IBlackServiceCollection: IDictionary<Type, ServiceDescriptor>
{
    public void AddSingleton<TService, TImplementation>()
        where TService : class where TImplementation : class, TService;
    public void AddSingleton<TService>() where TService : class;
    
    public void AddSingleton<TService>(TService service) where TService : class;
    public void AddSingleton<TService>(Func<IBlackServiceProvider, TService> factory) where TService : class;
    public void AddSingleton<TService>(Func<TService> factory) where TService : class;
    
    public void AddSingleton<TService, TImplementation>(Func<IBlackServiceProvider, TService> factory)
        where TService : class where TImplementation : class, TService;
    public void AddSingleton<TService, TImplementation>(Func<TService> factory)
        where TService : class where TImplementation : class, TService;
    public void AddSingleton(Type serviceType, Type implementationType);
    public void AddSingleton(Type serviceType);
    
    public void AddScoped<TService, TImplementation>()
        where TService : class where TImplementation : class, TService;
    
    public void AddScoped<TService, TImplementation>(Func<IBlackServiceProvider, TService> factory)
        where TService : class where TImplementation : class, TService;
    
    public void AddScoped<TService, TImplementation>(Func<TService> factory)
        where TService : class where TImplementation : class, TService;
    
    public void AddScoped<TService>() where TService : class;
    
    public void AddScoped<TService>(Func<IBlackServiceProvider, TService> factory) where TService : class;
    public void AddScoped<TService>(Func<TService> factory) where TService : class;
    public void AddScoped(Type serviceType, Type implementationType);
    public void AddScoped(Type serviceType);
    
    public void AddTransient<TService, TImplementation>()
        where TService : class where TImplementation : class, TService;
    public void AddTransient<TService>() where TService : class;

    public void AddTransient<TService>(Func<IBlackServiceProvider, TService> factory) where TService : class;
    public void AddTransient<TService>(Func<TService> factory) where TService : class;
    
    public void AddTransient<TService, TImplementation>(Func<IBlackServiceProvider, TService> factory)
        where TService : class where TImplementation : class, TService;
    
    public void AddTransient<TService, TImplementation>(Func<TService> factory)
        where TService : class where TImplementation : class, TService;
    
    public void AddTransient(Type serviceType, Type implementationType);
    public void AddTransient(Type serviceType);
}