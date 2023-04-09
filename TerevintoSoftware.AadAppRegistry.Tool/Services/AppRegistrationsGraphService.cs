using Microsoft.Graph;
using Microsoft.Graph.Applications.Item.AddPassword;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Spectre.Console;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IAppRegistrationsGraphService
{
    Task AddApiScopeAsync(Application application, string scopeName, string scopeDisplayName, string scopeDescription);
    Task<string> AddClientSecretAsync(Application application, DateTimeOffset? expirationTime);
    Task<Application> AddSpaRedirectUriAsync(Application application, Uri redirectUri);
    Task<Application> AddWebRedirectUriAsync(Application application, Uri redirectUri);
    Task<Application> CreateApplicationAsync(string displayName, SignInAudienceType signInAudienceType);
    Task<Application> GetApplicationByDisplayNameAsync(string displayName);
    Task<Application> GetApplicationByIdAsync(string clientId);
    Task SetApplicationIdUriAsync(Application application, string uri);
    Task<bool> ValidateConnectionAsync();
}

internal class AppRegistrationsGraphService : IAppRegistrationsGraphService
{
    private readonly GraphServiceClient _graphClient;

    public AppRegistrationsGraphService(IGraphServiceClientFactory graphServiceClientFactory)
    {
        _graphClient = graphServiceClientFactory.CreateClient();
    }

    public async Task<bool> ValidateConnectionAsync()
    {
        try
        {
            var randomClient = await _graphClient.Applications.GetAsync(r =>
            {
                r.QueryParameters.Top = 1;
                r.QueryParameters.Select = new[] { "appId" };
            });

            AnsiConsole.MarkupLine($"Connection to Microsoft Graph [bold green]successful[/].");

            return true;
        }
        catch (ODataError authError) when (authError.ResponseStatusCode == 403)
        {
            AnsiConsole.Write($"[bold red]Error: {authError.Error.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }

        return false;
    }

    public async Task<Application> GetApplicationByIdAsync(string clientId)
    {
        try
        {
            var apps = await _graphClient.Applications.GetAsync(r =>
            {
                r.QueryParameters.Filter = $"appId eq '{clientId}'";
                r.QueryParameters.Top = 1;
            });

            return apps.OdataCount == 1 ? apps.Value[0] : null;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return null;
        }
    }

    public async Task<Application> GetApplicationByDisplayNameAsync(string displayName)
    {
        try
        {
            var apps = await _graphClient.Applications.GetAsync(r =>
            {
                r.QueryParameters.Filter = $"displayName eq '{displayName}'";
                r.QueryParameters.Top = 1;
            });

            return apps.Value.Count == 1 ? apps.Value[0] : null;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return null;
        }
    }

    public async Task<Application> CreateApplicationAsync(string displayName, SignInAudienceType signInAudienceType)
    {
        var appToBeCreated = new Application
        {
            DisplayName = displayName,
            SignInAudience = signInAudienceType.ToString(),
        };

        return await _graphClient.Applications.PostAsync(appToBeCreated);
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
            
            await _graphClient.Applications[application.Id].PatchAsync(toUpdate);
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

            await _graphClient.Applications[application.Id].PatchAsync(updatedApp);
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

        var result = await _graphClient.Applications[application.Id].AddPassword.PostAsync(requestBody);

        return result.SecretText;
    }

    public async Task<Application> AddSpaRedirectUriAsync(Application application, Uri redirectUri)
    {
        application.Spa ??= new();
        application.Spa.RedirectUris ??= new();

        if (!application.Spa.RedirectUris.Any(x => x == redirectUri.ToString()))
        {
            application.Spa.RedirectUris.Add(redirectUri.ToString());

            return await _graphClient.Applications[application.Id].PatchAsync(application);
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

            return await _graphClient.Applications[application.Id].PatchAsync(application);
        }

        return application;
    }
}
