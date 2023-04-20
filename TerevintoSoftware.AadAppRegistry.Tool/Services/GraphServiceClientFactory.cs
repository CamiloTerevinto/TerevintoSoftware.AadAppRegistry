using Microsoft.Graph;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IGraphServiceClientFactory
{
    GraphServiceClient CreateClient();
}

internal class GraphServiceClientFactory : IGraphServiceClientFactory
{
    private readonly IConfigurationService _configurationService;

    public GraphServiceClientFactory(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public GraphServiceClient CreateClient()
    {
        return new(_configurationService.GetAzureCredential(), scopes: new[] { "https://graph.microsoft.com/.default" });
    }
}
