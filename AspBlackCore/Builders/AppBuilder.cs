using System.Text.Json;
using System.Threading.Channels;
using AspBlackCore.Config;
using AspBlackCore.Dispatchers;
using AspBlackCore.Handlers;
using BlackDependencyInjection;
using BlackDependencyInjection.Interfaces;
using WebServer;

namespace AspBlackCore.Builders;

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
        Services.AddSingleton(Services);
        
        Services.AddSingleton<MiddlewarePipelineBuilder>();
        Services.AddSingleton<MinimalApiRequestHandler>();
        Services.AddSingleton<MinimalApiDispatcher>();
        
        if (serviceProvider.GetService(typeof(ControllerDispatcher)) is null)
        {
            Services.AddSingleton<ControllerRequestHandler>();
            Services.AddSingleton<ControllerDispatcher>();
        }
        
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), AppSettings.FileName);
        Dictionary<string, JsonElement>? serialized;
        if (!File.Exists(filePath))
        {
            serialized = null;
        }
        else
        {
            var fileContent = File.ReadAllText(filePath);
            serialized = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(fileContent);
        }
        
        BlackWebServer server;
        
        var channel = Channel.CreateUnbounded<BlackWebServerRequestCell>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true
        });
        
        if (serialized is null)
        {
            server = new BlackWebServer(channel.Writer);
        }
        else
        {
            var settings = new AppSettings
            {
                Settings = serialized
            };
            var webServerSettings = settings.WebServerSettings;
            server = new BlackWebServer(webServerSettings.IpAddress, webServerSettings.Port,
                channel.Writer);
        }

        return new App(serviceProvider, server, channel.Reader);
    }
}