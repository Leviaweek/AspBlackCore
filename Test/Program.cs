using AspBlackCore.Builders;
using AspBlackCore.Extensions;
using DIAutoRegistration;

var builder = new AppBuilder();

builder.Services.RegisterAllServices();
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();

await app.StartAsync();