using Spectre.Console;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IAppConfigurationService
{
    Task<OperationResultStatus> AddAppScopeAsync(ConfigureAppAddScopeSettings settings);
    Task<OperationResultStatus> DeleteAppAsync(DeleteAppSettings settings);
}

internal class AppConfigurationService : IAppConfigurationService
{
    private readonly IGraphClientService _graphService;

    public AppConfigurationService(IGraphClientService graphService)
    {
        _graphService = graphService;
    }

    public async Task<OperationResultStatus> AddAppScopeAsync(ConfigureAppAddScopeSettings settings)
    {
        var application = await _graphService.GetApplicationByIdOrNameAsync(settings.ApplicationName);

        if (application == null)
        {
            AnsiConsole.MarkupLine("[bold red]Error:[/] the consumer application Id provided could not be found");
            return OperationResultStatus.NotFound;
        }

        var api = await _graphService.GetApplicationByIdOrNameAsync(settings.ApiAppId);

        if (api == null)
        {
            AnsiConsole.MarkupLine("[bold red]Error:[/] the API Id provided could not be found");
            return OperationResultStatus.NotFound;
        }

        var scope = api.Api.Oauth2PermissionScopes.FirstOrDefault(x => x.Value == settings.ScopeName);

        if (scope == null)
        {
            AnsiConsole.MarkupLine("[bold red]Error:[/] the scope name provided could not be found");
            return OperationResultStatus.NotFound;
        }

        await _graphService.AddConsumedScopeByIdAsync(application, api.AppId, scope.Id.Value);

        AnsiConsole.MarkupLine("[bold green]Success:[/] the scope was added to the application. Note that you need to grant admin approval for the scope.");

        return OperationResultStatus.Success;
    }

    public async Task<OperationResultStatus> DeleteAppAsync(DeleteAppSettings settings)
    {
        var application = await _graphService.GetApplicationByIdOrNameAsync(settings.ApplicationName);
        if (application == null)
        {
            AnsiConsole.MarkupLine("[bold red]Error:[/] the application Id provided could not be found");
            return OperationResultStatus.Failed;
        }

        await _graphService.DeleteApplicationAsync(application);
        AnsiConsole.MarkupLine("[bold green]Success:[/] the application was deleted.");

        return OperationResultStatus.Success;
    }
}
