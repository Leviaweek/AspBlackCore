using AspBlackCore.Delegates;
using AspBlackCore.Dispatchers;
using AspBlackCore.Interfaces;

namespace AspBlackCore.Middlewares;

internal sealed class ControllerMiddleware : Middleware
{
    private readonly ControllerDispatcher _apiDispatcher;
    public ControllerMiddleware(RequestDelegate next, ControllerDispatcher apiDispatcher) : base(next)
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