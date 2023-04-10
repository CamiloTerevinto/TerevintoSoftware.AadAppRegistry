using Microsoft.Graph;
using Microsoft.Graph.Applications.Item.AddPassword;
using Microsoft.Graph.Models;
using Spectre.Console;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IGraphClientService
{
    Task AddApiScopeAsync(Application application, string scopeName, string scopeDisplayName, string scopeDescription);
    Task<string> AddClientSecretAsync(Application application, DateTimeOffset? expirationTime);
    Task<Application> AddSpaRedirectUriAsync(Application application, Uri redirectUri);
    Task<Application> AddWebRedirectUriAsync(Application application, Uri redirectUri);
    Task<Application> CreateApplicationAsync(string displayName, SignInAudienceType signInAudienceType);
    Task<Application> GetApplicationByDisplayNameAsync(string displayName);
    Task<Application> GetApplicationByIdAsync(string clientId);
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

    public async Task<Application> GetApplicationByIdAsync(string clientId)
    {
        var apps = await GraphClient.Applications.GetAsync(r =>
        {
            r.QueryParameters.Filter = $"appId eq '{clientId}'";
            r.QueryParameters.Top = 1;
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

    public async Task<Application> AddSpaRedirectUriAsync(Application application, Uri redirectUri)
    {
        application.Spa ??= new();
        application.Spa.RedirectUris ??= new();

        if (!application.Spa.RedirectUris.Any(x => x == redirectUri.ToString()))
        {
            application.Spa.RedirectUris.Add(redirectUri.ToString());

            return await GraphClient.Applications[application.Id].PatchAsync(application);
        }

        return application;
    }

    public async Task<Application> AddWebRedirectUriAsync(Application application, Uri redirectUri)
    {
        application.Web ??= new();
        application.Web.RedirectUris ??= new();

        if (!application.Web.RedirectUris.Any(x => x == redirectUri.ToString()))
        {
            application.Web.RedirectUris.Add(redirectUri.ToString());

            return await GraphClient.Applications[application.Id].PatchAsync(application);
        }

        return application;
    }
}
