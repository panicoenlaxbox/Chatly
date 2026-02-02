using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Chatly;

public class Application(IConfiguration configuration, ILogger<Application> logger)
{
    public async Task RunAsync()
    {
        var azureEndpoint = configuration["Azure:Endpoint"] ?? throw new InvalidOperationException("Azure:Endpoint is not configured.");
        var apiKey = configuration["Azure:ApiKey"] ?? throw new InvalidOperationException("Azure:ApiKey is not configured.");
        var deploymentName = configuration["Azure:DeploymentName"] ?? throw new InvalidOperationException("Azure:DeploymentName is not configured.");        

        logger.LogInformation("Starting application with Azure Endpoint: {Endpoint}, Deployment: {Deployment}", azureEndpoint, deploymentName);

        IChatClient client =
            new ChatClientBuilder(
                new Azure.AI.OpenAI.AzureOpenAIClient(
                    new Uri(azureEndpoint),
                    new Azure.AzureKeyCredential(apiKey)).GetChatClient(deploymentName).AsIChatClient())
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

        Console.WriteLine("Write 'exit' to quit the application.");

        while (true)
        {
            Console.Write("User: ");
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

            Console.WriteLine($"Assistant: {assistantMessage}");
        }
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
