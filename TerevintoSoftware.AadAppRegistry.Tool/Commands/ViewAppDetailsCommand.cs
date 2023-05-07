using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

[ExcludeFromCodeCoverage]
internal class ViewAppDetailsCommand : AsyncCommand<ViewAppDetailsSettings>
{
    private readonly IGraphClientService _graphService;
    private readonly AppRegistryConfiguration _appRegistryConfiguration;

    public ViewAppDetailsCommand(IGraphClientService graphService, IConfigurationService configurationService)
    {
        _graphService = graphService;
        _appRegistryConfiguration = configurationService.Load();
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ViewAppDetailsSettings settings)
    {
        var result = await _graphService.GetApplicationByIdOrNameAsync(settings.ApplicationName);

        if (!result.Success)
        {
            AnsiConsole.MarkupLine($"[red]{result.Message}[/]");
            return 1;
        }

        var app = result.Data;
        var uri = app.IdentifierUris.FirstOrDefault() ?? "";
        var applicationUriBase = _appRegistryConfiguration.OperatingMode == OperatingMode.AzureB2C ?
            $"https://{_appRegistryConfiguration.TenantName}/" : "api://";

        var scopesToGet = app.RequiredResourceAccess.SelectMany(x => x.ResourceAccess.Select(y => (x.ResourceAppId, y.Id.Value))).ToList();
        var scopeNames = Array.Empty<string>();

        if (scopesToGet.Any())
        {
            var scopeNamesResult = await _graphService.GetScopeNamesAsync(scopesToGet);

            if (!scopeNamesResult.Success)
            {
                AnsiConsole.MarkupLine($"[red]{scopeNamesResult.Message}[/]");
                return 1;
            }

            scopeNames = scopeNamesResult.Data
                .Select(x => $"{applicationUriBase}{x.Item1}/{x.Item2}")
                .ToArray();
        }

        var data = new
        {
            ClientId = app.AppId,
            app.DisplayName,
            Uri = uri,
            Scopes = app.Api.Oauth2PermissionScopes
                .Select(s => $"{uri}/{s.Value}")
                .ToList(),
            RedirectUris = app.Web.RedirectUris.Select(u => "Web: " + u)
                .Concat(app.Spa.RedirectUris.Select(u => "SPA: " + u))
                .Concat(app.PublicClient.RedirectUris.Select(u => "Native: " + u))
                .ToList(),
            Secret = app.PasswordCredentials.Count > 0
                ? app.PasswordCredentials.All(x => x.EndDateTime > DateTime.Today) ? "Active" : "Expired"
                : "",
            ConsumedScopes = scopeNames
        };

        AnsiConsole.Write(new JsonText(JsonSerializer.Serialize(data)));

        return 0;
    }
}

