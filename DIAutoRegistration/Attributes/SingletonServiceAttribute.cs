using BlackDependencyInjection;

namespace DIAutoRegistration.Attributes;


[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SingletonServiceAttribute<T>() : ServiceAttribute(ServiceLifetime.Singleton, typeof(T));
    
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SingletonServiceAttribute() : ServiceAttribute(ServiceLifetime.Singleton, null);