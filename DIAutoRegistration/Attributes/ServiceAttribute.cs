namespace DIAutoRegistration.Attributes;

public abstract class ServiceAttribute : Attribute
{
    internal readonly Type? BaseType;

    protected ServiceAttribute(Type? baseType)
    {
        BaseType = baseType;
    }
}