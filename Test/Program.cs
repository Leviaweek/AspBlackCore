using System.Text.Json.Serialization;
using AspBlackCore;
using BlackDependencyInjection;
using BlackDependencyInjection.Interfaces;

var builder = new AppBuilder();

builder.Services.AddSingleton<IBlackServiceCollection, BlackServiceCollection>();
builder.Services.AddTransient<IBlackServiceProvider, RootBlackServiceProvider>();
builder.Services.AddTransient<TestType>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/", (CustomRequest request, string queryName, TestType test) =>
{
    Console.WriteLine(queryName);
    Console.WriteLine(request);
    Console.WriteLine(test);
    Console.WriteLine(test.ServiceProvider.GetType().FullName);
});

await app.StartAsync();


public sealed record CustomRequest([property: JsonPropertyName("name")]string Name,
    [property: JsonPropertyName("age")]int Age);

public sealed record TestType(IBlackServiceProvider ServiceProvider);
