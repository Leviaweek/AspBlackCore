namespace WebServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class ContentTypeHandlerAttribute : Attribute
{
    public string ContentType { get; }
    public ContentTypeHandlerAttribute(string contentType) => ContentType = contentType;
}