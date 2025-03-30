using BlackDependencyInjection;

namespace DIAutoRegistration.Attributes;


[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SingletonServiceAttribute(
    Type? baseType = null) : ServiceAttribute(ServiceLifetime.Singleton, baseType);