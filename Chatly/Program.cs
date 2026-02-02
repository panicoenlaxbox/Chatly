using Chatly;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration(config => 
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .AddUserSecrets<Program>());

builder.ConfigureServices(services =>
    services.AddLogging(logging => logging.AddConsole())
        .AddSingleton<Application>());

var host = builder.Build();

var application = host.Services.GetRequiredService<Application>();
await application.RunAsync();