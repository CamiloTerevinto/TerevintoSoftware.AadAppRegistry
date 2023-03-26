using Azure.Identity;
using Microsoft.Graph;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;

namespace TerevintoSoftware.AadAppRegistry.Tool.Utilities;

internal static class GraphServiceClientFactory
{
    internal static GraphServiceClient CreateClient(AppRegistryConfiguration authenticationOptions)
    {
        var creds= new ClientSecretCredential(authenticationOptions.ClientCredentials.TenantId, 
            authenticationOptions.ClientCredentials.ClientId, authenticationOptions.ClientCredentials.ClientSecret);

        return new GraphServiceClient(creds, scopes: new[] { "Application.ReadWrite.All" });
    }
}
