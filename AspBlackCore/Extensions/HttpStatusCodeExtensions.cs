using System.Reflection;
using AspBlackCore.Attributes;
using AspBlackCore.Enums;

namespace AspBlackCore.Extensions;

public static class HttpStatusCodeExtensions
{
    private static readonly Dictionary<HttpStatusCode, string> StatusDescriptions = new();
    
    static HttpStatusCodeExtensions()
    {
        var fields = typeof(HttpStatusCode).GetFields(BindingFlags.Public | BindingFlags.Static);
        
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<StatusDescriptionAttribute>();
            
            if (attribute is null) continue;
            
            if (!Enum.TryParse(field.Name, out HttpStatusCode statusCode)) continue;
            
            StatusDescriptions[statusCode] = attribute.Description;
        }
    }
    
    public static string GetDescription(this HttpStatusCode statusCode) => StatusDescriptions[statusCode];
}