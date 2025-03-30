namespace DIAutoRegistration.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class FactoryMethodAttribute : Attribute
{
    public const string Name = "Create";
}