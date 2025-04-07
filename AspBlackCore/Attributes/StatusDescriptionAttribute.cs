using AspBlackCore.Models;

namespace AspBlackCore.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class StatusDescriptionAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}

public abstract class ControllerAttribute : Attribute;


[AttributeUsage(AttributeTargets.Class)]
public sealed class RouteAttribute(string route = "/") : ControllerAttribute
{
    public string Route { get; } = route;
}

public class HttpMethodAttribute : ControllerAttribute
{
    public string Method { get; }
    public string Endpoint { get; }

    public HttpMethodAttribute(string method, string endpoint)
    {
        Method = method;
        Endpoint = endpoint;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class HttpGetAttribute(string endpoint = "") : HttpMethodAttribute(HttpMethods.Get, endpoint);

[AttributeUsage(AttributeTargets.Method)]
public sealed class HttpPostAttribute(string endpoint = "") : HttpMethodAttribute(HttpMethods.Post, endpoint);

[AttributeUsage(AttributeTargets.Method)]
public sealed class HttpPutAttribute(string endpoint = "") : HttpMethodAttribute(HttpMethods.Put, endpoint);

[AttributeUsage(AttributeTargets.Method)]
public sealed class HttpDeleteAttribute(string endpoint = "") : HttpMethodAttribute(HttpMethods.Delete, endpoint);