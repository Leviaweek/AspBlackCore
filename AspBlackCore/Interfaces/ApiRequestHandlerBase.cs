using System.Reflection;
using WebServer.Interfaces;

namespace AspBlackCore.Interfaces;

internal abstract class ApiRequestHandlerBase
{
    internal IHttpResponseBuilder CreateResponse(IHttpRequest request, IHttpResponseBuilder builder, MethodInfo handler,
        object? target = null)
    {
        var parameters = handler.GetParameters();

        var args = new object?[parameters.Length];

        if (!InsertParameters(parameters, request, args)) return builder.SetStatusCode(400);
        
        var responseType = handler.ReturnType;

        if (Nullable.GetUnderlyingType(responseType) != null)
        {
            throw new InvalidOperationException("Nullable types are not supported");
        }

        var responseObject = handler.Invoke(target, args);

        if (responseObject is Task task)
        {
            if (task.GetType().IsGenericType)
            {
                var result = task.GetType().GetProperty("Result")?.GetValue(task);
                responseObject = result;
            }
            else
            {
                ((Task)responseObject).Wait();
                return builder.SetStatusCode(200);
            }
        }

        return responseObject switch
        {
            IHttpResponseBuilder response => response,
            null => builder.SetStatusCode(400),
            _ => responseType == typeof(void)
                ? builder.SetStatusCode(200)
                : builder.SetStatusCode(200).SetBody(responseObject)
        };
    }
    
    internal abstract bool InsertParameters(ParameterInfo[] parameters, IHttpRequest request, object?[] args);
}