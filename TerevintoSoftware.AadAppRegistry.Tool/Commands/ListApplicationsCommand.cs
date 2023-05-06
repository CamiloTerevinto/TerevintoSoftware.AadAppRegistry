using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

[ExcludeFromCodeCoverage]
internal class ListApplicationsCommand : AsyncCommand<ListAppsSettings>
{
    private readonly IGraphClientService _graphService;

    public ListApplicationsCommand(IGraphClientService graphService)
    {
        _graphService = graphService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ListAppsSettings settings)
    {
        var result = await _graphService.GetApplicationsAsync(settings.Take, settings.OrderBy);

        if (!result.Success)
        {
            AnsiConsole.MarkupLine($"[red]{result.Message}[/]");
            return 1;
        }

        var table = new Table
        {
            Title = new TableTitle($"AAD Applications. Displaying {result.Data.Value.Count}/{result.Data.OdataCount}."),
        };

        table.Centered()
            .RoundedBorder()
            .SafeBorder();

        table.AddColumns(
            new TableColumn("[green]Application Id[/]").Centered(), 
            new TableColumn("[green]Display Name[/]").Centered(),
            new TableColumn("[green]Types[/]")
        );

        foreach (var app in result.Data.Value)
        {
            var uri = app.IdentifierUris.FirstOrDefault() ?? "";
            var types = new[] {
                app.Api.Oauth2PermissionScopes.Count > 0 ? "Api" : "",
                app.Web.RedirectUris.Count > 0 ? "Web" : "",
                app.Spa.RedirectUris.Count > 0 ? "SPA" : "",
                app.PublicClient.RedirectUris.Count > 0 ? "Public" : "",
                app.PasswordCredentials.Count > 0 ? 
                   app.PasswordCredentials.All(x => x.EndDateTime > DateTime.Today) ? "Confidential ([green]active[/])"  : "Confidential ([red]expired[/])"
                : "",
            };

            types = types.Where(t => t != "").ToArray();

            table.AddRow(new Text(app.AppId, new Style(link: $"{Constants.PortalOverviewUrl}{app.AppId}")), 
                         new Text(app.DisplayName), new Markup(string.Join("; ", types)));
        }

        AnsiConsole.Write(table);

        return 0;
    }
}

