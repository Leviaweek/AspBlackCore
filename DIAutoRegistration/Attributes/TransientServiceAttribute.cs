namespace DIAutoRegistration.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]

public sealed class TransientServiceAttribute(
    Type? baseType = null) : ServiceAttribute(baseType);