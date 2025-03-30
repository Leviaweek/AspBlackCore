namespace BlackDependencyInjection.Interfaces;

public interface IBlackServiceCollection: IDictionary<Type, ServiceDescriptor>
{
    
    public void AddSingleton<TService>(TService service) where TService : class;
    public void AddSingleton<TService>(Func<IBlackServiceProvider, TService>? factory = null) where TService : class;
    
    public void AddSingleton<TService, TImplementation>(Func<IBlackServiceProvider, TService>? factory = null)
        where TService : class where TImplementation : class, TService;
    public void AddSingleton(Type serviceType, Type implementationType, Func<IBlackServiceProvider, object>? factory = null);
    public void AddSingleton(Type serviceType, Func<IBlackServiceProvider, object>? factory = null);
    
    
    public void AddScoped<TService, TImplementation>(Func<IBlackServiceProvider, TService>? factory = null)
        where TService : class where TImplementation : class, TService;
    
    
    public void AddScoped<TService>(Func<IBlackServiceProvider, TService>? factory = null) where TService : class;
    public void AddScoped(Type serviceType, Type implementationType, Func<IBlackServiceProvider, object>? factory = null);
    public void AddScoped(Type serviceType, Func<IBlackServiceProvider, object>? factory = null);
    

    public void AddTransient<TService>(Func<IBlackServiceProvider, TService>? factory = null) where TService : class;
    
    public void AddTransient<TService, TImplementation>(Func<IBlackServiceProvider, TService>? factory = null)
        where TService : class where TImplementation : class, TService;
    
    
    public void AddTransient(Type serviceType, Type implementationType, Func<IBlackServiceProvider, object>? factory = null);
    public void AddTransient(Type serviceType, Func<IBlackServiceProvider, object>? factory = null);
}