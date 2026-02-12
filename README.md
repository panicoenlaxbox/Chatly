# Chatly

A simple .NET console application for interacting with Azure OpenAI chat models using Microsoft.Extensions.AI.

## Features

- Interactive menu-driven interface using Spectre.Console
- Chat with Azure OpenAI models
- Function calling support (weather information)
- Colorful and user-friendly console UI

## Prerequisites

- .NET 10.0 SDK
- Azure OpenAI resource with a deployed chat model

## Configuration

Update the `appsettings.json` file with your Azure OpenAI credentials:

```json
{
  "Azure": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-api-key",
    "DeploymentName": "your-deployment-name"
  }
}
```

## Usage

1. Run the application: `dotnet run --project Chatly/Chatly.csproj`
2. Select "Start Chat" from the main menu
3. Type your messages and interact with the AI assistant
4. Type "exit" to return to the main menu
5. Select "Exit" to close the application