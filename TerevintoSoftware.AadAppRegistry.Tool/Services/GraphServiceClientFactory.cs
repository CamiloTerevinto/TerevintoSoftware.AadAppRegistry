using Azure.Identity;
using Microsoft.Graph;
using TerevintoSoftware.AadAppRegistry.Tool.Models;

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
        var clientCredentials = _configurationService.Load().ClientCredentials;

        if (!clientCredentials.IsValid())
        {
            throw new InvalidCredentialsException();
        }

        var creds = new ClientSecretCredential(clientCredentials.TenantId,
            clientCredentials.ClientId, clientCredentials.ClientSecret);

        return new(creds, scopes: new[] { "https://graph.microsoft.com/.default" });
    }
}
