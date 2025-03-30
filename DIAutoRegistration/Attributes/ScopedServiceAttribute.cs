namespace DIAutoRegistration.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ScopedServiceAttribute(
    Type? baseType = null) : ServiceAttribute(baseType);