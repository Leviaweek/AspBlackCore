using System.Text.Json.Serialization;
using AspBlackCore;
using BlackDependencyInjection.Interfaces;
using DIAutoRegistration;
using DIAutoRegistration.Attributes;

var builder = new AppBuilder();

builder.Services.RegisterAllServices();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/", (CustomRequest request, string queryName, ITestType test) =>
{
    Console.WriteLine(queryName);
    Console.WriteLine(request);
    Console.WriteLine(test);
    Console.WriteLine(test.ServiceProvider.GetType().FullName);
});

await app.StartAsync();


public sealed record CustomRequest([property: JsonPropertyName("name")]string Name,
    [property: JsonPropertyName("age")]int Age);


[TransientService(typeof(ITestType))]
public sealed record TestType(IBlackServiceProvider ServiceProvider): ITestType
{
    [FactoryMethod]
    public static TestType Create(IBlackServiceProvider serviceProvider)
    {
        Console.WriteLine("Creating TestType");
        return new TestType(serviceProvider);
    }
}

public interface ITestType
{
    IBlackServiceProvider ServiceProvider { get; }
}