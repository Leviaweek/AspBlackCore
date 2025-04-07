using BlackDependencyInjection.Base;

namespace DIAutoRegistration.Attributes;

public abstract class ServiceAttribute : Attribute
{
    internal readonly Type? BaseType;
    internal readonly ServiceLifetime Lifetime;

    protected ServiceAttribute(ServiceLifetime lifetime, Type? baseType)
    {
        Lifetime = lifetime;
        BaseType = baseType;
    }
}