using APIGateway.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppAuthentication();

var filePath = Path.Combine(AppContext.BaseDirectory, "ocelot.json");
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(filePath, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddOcelot();

var app = builder.Build();
try
{
    await app.UseOcelot();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
app.Run();