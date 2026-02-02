# Chatly

A simple .NET console application for interacting with Azure OpenAI chat models using Microsoft.Extensions.AI.

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