using Microsoft.Graph;
using Microsoft.Graph.Applications.Item.AddPassword;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Spectre.Console;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal class AppRegistrationsGraphService
{
    private readonly GraphServiceClient _graphClient;

    public AppRegistrationsGraphService(GraphServiceClient graphServiceClient)
    {
        _graphClient = graphServiceClient;
    }

    public async Task ValidateConnection()
    {
        try
        {
            var randomClient = await _graphClient.Applications.GetAsync(r => r.QueryParameters.Top = 1);

            AnsiConsole.MarkupLine($"Connection to Microsoft Graph [bold green]successful[/].");
        }
        catch (ODataError authError) when (authError.ResponseStatusCode == 403)
        {
            AnsiConsole.Write($"[bold red]Error: {authError.Error.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    public async Task<Application> GetApplicationById(string clientId)
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

    public async Task<Application> GetApplicationByDisplayName(string displayName)
    {
        try
        {
            var apps = await _graphClient.Applications.GetAsync(r =>
            {
                r.QueryParameters.Filter = $"displayName eq '{displayName}'";
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

    public async Task<Application> CreateApplication(string displayName)
    {
        var app = await GetApplicationByDisplayName(displayName);

        if (app != null)
        {
            return app;
        }

        try
        {
            app = new Application
            {
                DisplayName = displayName
            };

            await _graphClient.Applications.PostAsync(app);

            return app;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return null;
        }
    }

    public async Task SetApplicationIdUri(Application application, string uri)
    {
        application.IdentifierUris ??= new();

        if (!application.IdentifierUris.Any(x => x == uri))
        {
            application.IdentifierUris.Add(uri);

            try
            {
                await _graphClient.Applications[application.Id].PatchAsync(application);

                AnsiConsole.MarkupLine($"[yellow]{application.AppId}[/]: ApplicationId URI added - {uri}");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }
    }

    public async Task AddApiScope(Application application, string scopeName, string friendlyScopeName, string description)
    {
        application.Api ??= new();
        application.Api.Oauth2PermissionScopes ??= new();

        if (!application.Api.Oauth2PermissionScopes.Any(x => x.Value == scopeName))
        {
            application.Api.Oauth2PermissionScopes.Add(new PermissionScope
            {
                UserConsentDisplayName = friendlyScopeName,
                UserConsentDescription = description,
                AdminConsentDisplayName = friendlyScopeName,
                AdminConsentDescription = description,
                IsEnabled = true
            });

            try
            {
                await _graphClient.Applications[application.Id].PatchAsync(application);

                AnsiConsole.MarkupLine($"[yellow]{application.AppId}[/]: API scope added - {scopeName}");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }
    }

    public async Task AddClientSecretAsync(Application application, DateTimeOffset? expirationTime)
    {
        var requestBody = new AddPasswordPostRequestBody
        {
            PasswordCredential = new PasswordCredential
            {
                DisplayName = "Client secret",
                EndDateTime = expirationTime ?? DateTimeOffset.UtcNow.AddMonths(6)
            }
        };

        try
        {
            var result = await _graphClient.Applications[application.Id].AddPassword.PostAsync(requestBody);

            AnsiConsole.MarkupLine($"[yellow]{application.AppId}[/]: secret generated: [green]{result.SecretText}[/]. [bold red]Do not close this window until you have copied the Secret as you will not be able to get the value again.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    public async Task AddSpaRedirectUri(Application application, Uri redirectUri)
    {
        application.Spa ??= new();
        application.Spa.RedirectUris ??= new();

        if (!application.Spa.RedirectUris.Any(x => x == redirectUri.ToString()))
        {
            application.Spa.RedirectUris.Add(redirectUri.ToString());

            try
            {
                await _graphClient.Applications[application.Id].PatchAsync(application);

                AnsiConsole.MarkupLine($"[yellow]{application.AppId}[/]: SPA redirect uri added - {redirectUri}");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }
    }

    public async Task AddWebRedirectUri(Application application, Uri redirectUri)
    {
        application.Web ??= new();
        application.Web.RedirectUris ??= new();

        if (!application.Web.RedirectUris.Any(x => x == redirectUri.ToString()))
        {
            application.Web.RedirectUris.Add(redirectUri.ToString());

            try
            {
                await _graphClient.Applications[application.Id].PatchAsync(application);

                AnsiConsole.MarkupLine($"[yellow]{application.AppId}[/]: Web redirect uri added - {redirectUri}");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }
    }
}
