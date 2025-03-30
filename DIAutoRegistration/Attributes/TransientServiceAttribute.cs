using BlackDependencyInjection;

namespace DIAutoRegistration.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class TransientServiceAttribute<T>() : ServiceAttribute(ServiceLifetime.Transient, typeof(T));


[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class TransientServiceAttribute() : ServiceAttribute(ServiceLifetime.Transient, null);