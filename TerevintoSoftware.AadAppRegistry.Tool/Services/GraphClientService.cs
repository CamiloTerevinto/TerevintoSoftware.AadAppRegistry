using Microsoft.Graph;
using Microsoft.Graph.Applications.Item.AddPassword;
using Microsoft.Graph.Models;
using Spectre.Console;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IGraphClientService
{
    Task AddConsumedScopeByIdAsync(Application application, string originalAppId, Guid scopeId);
    Task AddApiScopeAsync(Application application, string scopeName, string scopeDisplayName, string scopeDescription);
    Task<string> AddClientSecretAsync(Application application, DateTimeOffset? expirationTime);
    Task AddNativeRedirectUrisAsync(Application application, string[] redirectUris);
    Task AddSpaRedirectUrisAsync(Application application, string[] redirectUris);
    Task AddWebRedirectUrisAsync(Application application, string[] redirectUris);
    Task<Application> CreateApplicationAsync(string displayName, SignInAudienceType signInAudienceType);
    Task<Application> GetApplicationByDisplayNameAsync(string displayName);
    Task<Application> GetApplicationByIdAsync(string clientId);
    Task<Application> GetApplicationByIdOrNameAsync(string clientIdOrDisplayName);
    Task SetApplicationIdUriAsync(Application application, string uri);
    Task ValidateConnectionAsync();
}

internal class GraphClientService : IGraphClientService
{
    private readonly IGraphServiceClientFactory _factory;

    private GraphServiceClient GraphClient => _factory.CreateClient();

    public GraphClientService(IGraphServiceClientFactory graphServiceClientFactory)
    {
        _factory = graphServiceClientFactory;
    }

    public async Task ValidateConnectionAsync()
    {
        var randomClient = await GraphClient.Applications.GetAsync(r =>
        {
            r.QueryParameters.Top = 1;
            r.QueryParameters.Select = new[] { "appId" };
        });

        AnsiConsole.MarkupLine($"[bold green]Success:[/] connection to Microsoft Graph was successful.");
    }

    public async Task<Application> GetApplicationByIdOrNameAsync(string clientIdOrDisplayName)
    {
        return await GetApplicationByIdAsync(clientIdOrDisplayName) ?? await GetApplicationByDisplayNameAsync(clientIdOrDisplayName);
    }

    public async Task<Application> GetApplicationByIdAsync(string clientId)
    {
        var apps = await GraphClient.Applications.GetAsync(r =>
        {
            r.QueryParameters.Filter = $"appId eq '{clientId}'";
            r.QueryParameters.Top = 1;
            r.Headers.Add("ConsistencyLevel", "eventual");
            r.QueryParameters.Count = true;
        });

        return apps.Value.Count == 1 ? apps.Value[0] : null;
    }

    public async Task<Application> GetApplicationByDisplayNameAsync(string displayName)
    {
        var apps = await GraphClient.Applications.GetAsync(r =>
        {
            r.QueryParameters.Filter = $"displayName eq '{displayName}'";
            r.QueryParameters.Top = 1;
        });

        return apps.Value.Count == 1 ? apps.Value[0] : null;
    }

    public async Task<Application> CreateApplicationAsync(string displayName, SignInAudienceType signInAudienceType)
    {
        var appToBeCreated = new Application
        {
            DisplayName = displayName,
            SignInAudience = signInAudienceType.ToString(),
        };

        return await GraphClient.Applications.PostAsync(appToBeCreated);
    }

    public async Task SetApplicationIdUriAsync(Application application, string uri)
    {
        application.IdentifierUris ??= new();

        if (!application.IdentifierUris.Any())
        {
            var toUpdate = new Application
            {
                IdentifierUris = new() { uri }
            };

            await GraphClient.Applications[application.Id].PatchAsync(toUpdate);
        }
    }

    public async Task AddApiScopeAsync(Application application, string scopeName, string scopeDisplayName, string scopeDescription)
    {
        application.Api ??= new();
        application.Api.Oauth2PermissionScopes ??= new();

        if (!application.Api.Oauth2PermissionScopes.Any(x => x.Value == scopeName))
        {
            var updatedApp = new Application
            {
                Api = new ApiApplication
                {
                    Oauth2PermissionScopes = new()
                    {
                        new PermissionScope
                        {
                            AdminConsentDisplayName = scopeDisplayName,
                            AdminConsentDescription = scopeDescription,
                            Value = scopeName,
                            IsEnabled = true,
                            Id = Guid.NewGuid(),
                            Type = "Admin"
                        }
                    }
                }
            };

            await GraphClient.Applications[application.Id].PatchAsync(updatedApp);
        }
    }

    public async Task AddSpaRedirectUrisAsync(Application application, string[] redirectUris)
    {
        application.Spa ??= new();
        application.Spa.RedirectUris ??= new();

        var redirectUrisToSet = application.Spa.RedirectUris.Union(redirectUris).ToHashSet();

        if (redirectUris.Any())
        {
            var toUpdate = new Application
            {
                Spa = new SpaApplication
                {
                    RedirectUris = redirectUrisToSet.ToList(),
                }
            };

            await GraphClient.Applications[application.Id].PatchAsync(toUpdate);
        }
    }

    public async Task AddWebRedirectUrisAsync(Application application, string[] redirectUris)
    {
        application.Web ??= new();
        application.Web.RedirectUris ??= new();

        var redirectUrisToSet = application.Web.RedirectUris.Union(redirectUris).ToHashSet();

        if (redirectUris.Any())
        {
            var toUpdate = new Application
            {
                Web = new WebApplication
                {
                    RedirectUris = redirectUrisToSet.ToList(),
                }
            };

            await GraphClient.Applications[application.Id].PatchAsync(toUpdate);
        }
    }

    public async Task AddNativeRedirectUrisAsync(Application application, string[] redirectUris)
    {
        application.PublicClient ??= new();
        application.PublicClient.RedirectUris ??= new();

        var redirectUrisToSet = application.PublicClient.RedirectUris.Union(redirectUris).ToHashSet();

        if (redirectUris.Any())
        {
            var toUpdate = new Application
            {
                PublicClient = new PublicClientApplication
                {
                    RedirectUris = redirectUrisToSet.ToList(),
                }
            };

            await GraphClient.Applications[application.Id].PatchAsync(toUpdate);
        }
    }

    public async Task<string> AddClientSecretAsync(Application application, DateTimeOffset? expirationTime)
    {
        var requestBody = new AddPasswordPostRequestBody
        {
            PasswordCredential = new PasswordCredential
            {
                DisplayName = "Client secret",
                EndDateTime = expirationTime ?? DateTimeOffset.UtcNow.AddMonths(6)
            }
        };

        var result = await GraphClient.Applications[application.Id].AddPassword.PostAsync(requestBody);

        return result.SecretText;
    }

    public async Task AddConsumedScopeByIdAsync(Application application, string originalAppId, Guid scopeId)
    {
        application.RequiredResourceAccess ??= new();

        if (application.RequiredResourceAccess.Any(x => x.ResourceAppId == originalAppId && x.ResourceAccess.Any(y => y.Id == scopeId)))
        {
            return;
        }

        var updatedApp = new Application
        {
            RequiredResourceAccess = new()
            {
                new()
                {
                    ResourceAppId = originalAppId,
                    ResourceAccess = new()
                    {
                        new()
                        {
                            Id = scopeId,
                            Type = "Scope",
                        }
                    }
                }
            }
        };

        await GraphClient.Applications[application.Id].PatchAsync(updatedApp);
    }
}
