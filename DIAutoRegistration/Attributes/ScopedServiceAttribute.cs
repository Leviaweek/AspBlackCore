using BlackDependencyInjection;

namespace DIAutoRegistration.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ScopedServiceAttribute(
    Type? baseType = null) : ServiceAttribute(ServiceLifetime.Scoped, baseType);