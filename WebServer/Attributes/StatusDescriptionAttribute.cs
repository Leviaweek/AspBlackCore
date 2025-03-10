namespace WebServer.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class StatusDescriptionAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}