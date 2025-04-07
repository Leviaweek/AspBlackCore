using System.Reflection;
using System.Text.Json;
using AspBlackCore.Interfaces;
using BlackDependencyInjection.Interfaces;
using WebServer.Interfaces;

namespace AspBlackCore.Handlers;

internal sealed class ControllerRequestHandler: ApiRequestHandlerBase
{
    private readonly IBlackServiceProvider _serviceProvider;

    public ControllerRequestHandler(IBlackServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    internal override bool InsertParameters(ParameterInfo[] parameters, IHttpRequest request, object?[] args)
    {
        var isBodyUsed = false;
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            var name = parameter.Name;
                
            if (name is not null)
            {
                if (request.Queries.TryGetValue(name, out var value) && parameter.ParameterType == typeof(string))
                {
                    args[i] = value;
                    continue;
                }
            }
            
            if (parameter.ParameterType == typeof(CancellationToken))
            {
                args[i] = _serviceProvider.GetRequiredService<BlackAppContext>().CancellationTokenSource.Token;
                continue;
            }
            
            if (isBodyUsed) return false;
            try
            {
                args[i] = request.GetBody(parameter.ParameterType);
            }
            catch (JsonException)
            {
                return false;
            }
            isBodyUsed = true;
        }

        return true;
    }
}