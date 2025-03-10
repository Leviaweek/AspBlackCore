using System.Text.Json;
using BlackDependencyInjection;
using BlackDependencyInjection.Interfaces;

namespace AspBlackCore;

public sealed class AppBuilder
{
    public readonly IBlackServiceCollection Services;

    public AppBuilder(IBlackServiceCollection services)
    {
        Services = services;
    }
    public AppBuilder() : this(new BlackServiceCollection()) { }
    
    public AppBuilder ConfigureServices(Action<IBlackServiceCollection> configureServices)
    {
        configureServices(Services);
        return this;
    }
    
    public App Build()
    {
        var serviceProvider = new RootBlackServiceProvider(Services);
        Services.AddSingleton<IBlackServiceProvider>(serviceProvider);
        Services.AddScoped<CancellationTokenSource>();

        var serialized = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(
            Path.Combine(Directory.GetCurrentDirectory(), AppSettings.FileName)));

        WebServer.WebServer server;
        if (serialized is null)
        {
            server = new WebServer.WebServer(serviceProvider);
        }
        else
        {
            var settings = new AppSettings
            {
                Settings = serialized
            };
            var webServerSettings = settings.WebServerSettings;
            server = new WebServer.WebServer(webServerSettings.IpAddress, webServerSettings.Port, serviceProvider);
        }

        return new App(serviceProvider, server);
    }
}