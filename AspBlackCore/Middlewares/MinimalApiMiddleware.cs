using AspBlackCore.Delegates;
using AspBlackCore.Dispatchers;
using AspBlackCore.Interfaces;

namespace AspBlackCore.Middlewares;

internal sealed class MinimalApiMiddleware : Middleware
{
    private readonly MinimalApiDispatcher _apiDispatcher;
    public MinimalApiMiddleware(RequestDelegate next, MinimalApiDispatcher apiDispatcher) : base(next)
    {
        _apiDispatcher = apiDispatcher;
    }
    
    public override async Task InvokeAsync(HttpContext context)
    {
        if (await _apiDispatcher.DispatchAsync(context))
        {
            return;
        }
        await Next(context);
    }
}