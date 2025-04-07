using System.Reflection;
using AspBlackCore.Attributes;
using AspBlackCore.Dispatchers;
using AspBlackCore.Handlers;
using AspBlackCore.Interfaces;
using BlackDependencyInjection.Interfaces;

namespace AspBlackCore.Extensions;

public static class BlackServiceCollectionExtensions
{
    public static void AddControllers(this IBlackServiceCollection services)
    {
        services.AddSingleton<ControllerDispatcher>(p =>
        {
            var dispatcher = new ControllerDispatcher(p.GetRequiredService<MinimalApiRequestHandler>(), p);
            
            var controllers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(ControllerBase).IsAssignableFrom(t));

            foreach (var controller in controllers)
            {
                var routeAttribute = controller.GetCustomAttribute<RouteAttribute>();

                if (routeAttribute is null) continue;

                var route = routeAttribute.Route;

                var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.GetCustomAttribute<HttpMethodAttribute>() is not null);
                services.AddScoped(controller);
                foreach (var method in methods)
                {
                    var httpMethodAttribute = method.GetCustomAttribute<HttpMethodAttribute>();

                    if (httpMethodAttribute is null) continue;

                    var httpMethod = httpMethodAttribute.Method;

                    var endpoint = Path.Combine(route, httpMethodAttribute.Endpoint);

                    dispatcher.Map(httpMethod, endpoint, method, controller);
                }
            }
            return dispatcher;
        });

        services.AddSingleton<ApiRequestHandlerBase, ControllerRequestHandler>();
    }
}