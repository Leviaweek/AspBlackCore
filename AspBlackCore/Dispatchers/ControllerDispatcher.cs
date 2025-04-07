using System.Reflection;
using AspBlackCore.Handlers;
using AspBlackCore.Models;
using BlackDependencyInjection.Interfaces;

namespace AspBlackCore.Dispatchers;

internal sealed class ControllerDispatcher
{
    private readonly MinimalApiRequestHandler _handler;
    private readonly IBlackServiceProvider _provider;
    private readonly Dictionary<MethodEndpoint, ControllerData> _maps = new();

    public ControllerDispatcher(MinimalApiRequestHandler handler, IBlackServiceProvider provider)
    {
        _handler = handler;
        _provider = provider;
    }
    public void Map(string method, string path, MethodInfo methodInfo, Type controllerType)
    {
        var endpoint = new MethodEndpoint(method, path);
        if (!_maps.TryAdd(endpoint, new ControllerData(methodInfo, controllerType)))
        {
            throw new InvalidOperationException($"Endpoint {method} {path} already exists");
        }
    }
    
    public async Task<bool> DispatchAsync(HttpContext context)
    {
        await Task.Yield();
        var methodEndpoint = new MethodEndpoint(context.Request.Method, context.Request.Path);
        
        if (_maps.TryGetValue(methodEndpoint, out var handler))
        {
            using var scope = _provider.CreateScope();
            var controller = scope.ServiceProvider.GetService(handler.ControllerType);
            
            if (controller is null) throw new InvalidOperationException($"Controller {handler.ControllerType} not found");
            
            context.Response = _handler.CreateResponse(context.Request, context.Response, handler.Info,
                controller);
            return true;
        }
        
        context.Response.SetStatusCode(404);
        return false;
    }
}

internal record struct ControllerData(MethodInfo Info, Type ControllerType);