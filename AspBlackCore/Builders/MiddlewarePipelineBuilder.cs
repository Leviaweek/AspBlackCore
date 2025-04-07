using AspBlackCore.Delegates;
using AspBlackCore.Interfaces;
using BlackDependencyInjection.Exceptions;
using BlackDependencyInjection.Interfaces;

namespace AspBlackCore.Builders;

internal sealed class MiddlewarePipelineBuilder
{
    private readonly IBlackServiceProvider _serviceProvider;
    private List<Func<RequestDelegate, RequestDelegate>> _middlewares = [];

    public MiddlewarePipelineBuilder(IBlackServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void Use<T>() where T: Middleware
    {
        _middlewares.Add(next =>
        {
            return async context =>
            {
                var middleware = GetMiddleware<T>(next);

                await middleware.InvokeAsync(context);
            };
        });
    }
    
    public RequestDelegate Build()
    {
        RequestDelegate pipeline = _ => Task.CompletedTask;
        
        for (var i = _middlewares.Count - 1; i >= 0; i--)
        {
            pipeline = _middlewares[i](pipeline);
        }
        
        return pipeline;
    }

    private Middleware GetMiddleware<T>(RequestDelegate next) where T : Middleware
    {
        var constructor = typeof(T).GetConstructors().First();
        
        var parameters = constructor.GetParameters();
                
        var args = new object?[parameters.Length];
                
        for (var i = _middlewares.Count - 1; i >= 0; i--)
        {
            var parameter = parameters[i];
            if (parameter.ParameterType == typeof(RequestDelegate))
            {
                args[i] = next;
                continue;
            }
                    
            var service = _serviceProvider.GetService(parameter.ParameterType);

            args[i] = service ?? throw new ServiceNotFoundException(parameter.ParameterType);
        }
                
        var middleware = (T)constructor.Invoke(args);
        return middleware;
    }
}