using BlackDependencyInjection;

namespace DIAutoRegistration.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ScopedServiceAttribute<T>() : ServiceAttribute(ServiceLifetime.Scoped, typeof(T));

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ScopedServiceAttribute() : ServiceAttribute(ServiceLifetime.Scoped, null);