using AspBlackCore.Handlers;
using AspBlackCore.Models;

namespace AspBlackCore.Dispatchers;

internal sealed class MinimalApiDispatcher
{
    private readonly MinimalApiRequestHandler _handler;
    private readonly Dictionary<MethodEndpoint, Delegate> _maps = new();

    public MinimalApiDispatcher(MinimalApiRequestHandler handler)
    {
        _handler = handler;
    }
    public void Map(string method, string path, Delegate handler)
    {
        var endpoint = new MethodEndpoint(method, path);
        if (!_maps.TryAdd(endpoint, handler))
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
            context.Response = _handler.CreateResponse(context.Request, context.Response, handler.Method);
            return true;
        }
        
        context.Response.SetStatusCode(404);
        return false;
    }
}