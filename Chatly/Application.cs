using System.ClientModel.Primitives;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Chatly;

public class Application(IConfiguration configuration, ILogger<Application> logger)
{
    public async Task RunAsync()
    {
        AnsiConsole.Write(
            new FigletText("Chatly")
                .Centered()
                .Color(Color.Blue));

        AnsiConsole.MarkupLine("[dim]A simple .NET console application for interacting with Azure OpenAI[/]");
        AnsiConsole.WriteLine();

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]What would you like to do?[/]")
                    .AddChoices(new[] {
                        "Start Chat",
                        "Exit"
                    }));

            if (choice == "Exit")
            {
                AnsiConsole.MarkupLine("[yellow]Goodbye![/]");
                break;
            }

            if (choice == "Start Chat")
            {
                await StartChatAsync();
            }
        }
    }

    private async Task StartChatAsync()
    {
        var azureEndpoint = configuration["Azure:Endpoint"] ?? throw new InvalidOperationException("Azure:Endpoint is not configured.");
        var apiKey = configuration["Azure:ApiKey"] ?? throw new InvalidOperationException("Azure:ApiKey is not configured.");
        var deploymentName = configuration["Azure:DeploymentName"] ?? throw new InvalidOperationException("Azure:DeploymentName is not configured.");

        logger.LogInformation("Starting application with Azure Endpoint: {Endpoint}, Deployment: {Deployment}", azureEndpoint, deploymentName);

        using var socketsHttpHandler = new SocketsHttpHandler();
        using var httpFileHandler = new HttpFileHandler("log.http",
        // ["api-key"]
        []
        )
        {
            InnerHandler = socketsHttpHandler
        };

        using var httpClient = new HttpClient(httpFileHandler);
        PipelineTransport transport = new HttpClientPipelineTransport(httpClient);

        using IChatClient client =
            new ChatClientBuilder(
                new Azure.AI.OpenAI.AzureOpenAIClient(
                    new Uri(azureEndpoint),
                    new Azure.AzureKeyCredential(apiKey),
                    new Azure.AI.OpenAI.AzureOpenAIClientOptions()
                    {
                        Transport = transport
                    }
                ).GetChatClient(deploymentName)
                .AsIChatClient())
            .UseFunctionInvocation()
            .Build();

        ChatOptions options = new()
        {
            Tools = [
                AIFunctionFactory.Create((string location) => GetWeather(location), "get_weather", "Get the current weather for a given location")
            ]
        };

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, """
            You are a helpful assistant. You can provide weather information when asked.
            """)
        };

        AnsiConsole.WriteLine();
        var panel = new Panel("[blue]Chat Session Started[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Blue);
        AnsiConsole.Write(panel);
        AnsiConsole.MarkupLine("[dim]Type 'exit' to return to the main menu.[/]");
        AnsiConsole.WriteLine();

        while (true)
        {
            AnsiConsole.Markup("[green]User:[/] ");
            var userMessage = (Console.ReadLine() ?? string.Empty).Trim();

            if (userMessage.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                continue;
            }

            messages.Add(new ChatMessage(ChatRole.User, userMessage));

            ChatResponse? response = await client.GetResponseAsync(messages, options);
            var assistantMessage = response.Text;

            messages.Add(new ChatMessage(ChatRole.Assistant, assistantMessage));

            AnsiConsole.MarkupLine($"[blue]Assistant:[/] {assistantMessage}");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[yellow]Chat session ended.[/]");
        AnsiConsole.WriteLine();
    }

    private string GetWeather(string location)
    {
        logger.LogInformation("Getting weather for location: {Location}", location);
        var weathers = new[] { "sunny", "cloudy", "rainy", "windy", "stormy" };
        var random = new Random();
        var weather = weathers[random.Next(weathers.Length)];
        return $"The weather in {location} is {weather}.";
    }
}
