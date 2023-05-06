using Microsoft.Graph.Models;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IAppConfigurationService
{
    Task<ServiceOperationResult> AddAppScopeAsync(ConfigureAppAddScopeSettings settings);
    Task<ServiceOperationResult> DeleteAppAsync(DeleteAppSettings settings);
}

internal class AppConfigurationService : IAppConfigurationService
{
    private readonly IGraphClientService _graphService;

    public AppConfigurationService(IGraphClientService graphService)
    {
        _graphService = graphService;
    }

    public async Task<ServiceOperationResult> AddAppScopeAsync(ConfigureAppAddScopeSettings settings)
    {
        var getAppResult = await EnsureApplicationExistsAsync(settings.ApiAppId);

        if (!getAppResult.Success)
        {
            return getAppResult;
        }

        var application = getAppResult.Data;
        var getApiResult = await EnsureApplicationExistsAsync(settings.ApiAppId);

        if (!getApiResult.Success)
        {
            return getApiResult;
        }

        var api = getApiResult.Data;
        var scope = api.Api.Oauth2PermissionScopes.FirstOrDefault(x => x.Value == settings.ScopeName);

        if (scope == null)
        {
            return new(ServiceOperationResultStatus.NotFound, "The scope name provided could not be found");
        }

        var addScopeResult = await _graphService.AddConsumedScopeByIdAsync(application, api.AppId, scope.Id.Value);

        if (!addScopeResult.Success)
        {
            return addScopeResult;
        }

        var link = Constants.PortalApiPermissionsUrl + application.AppId;

        return new(ServiceOperationResultStatus.Success, $"The scope was added to the application, but you need to grant admin approval here: {link}");
    }

    public async Task<ServiceOperationResult> DeleteAppAsync(DeleteAppSettings settings)
    {
        var getApplicationResult = await EnsureApplicationExistsAsync(settings.ApplicationName);

        if (!getApplicationResult.Success)
        {
            return getApplicationResult;
        }

        var deleteAppResult = await _graphService.DeleteApplicationAsync(getApplicationResult.Data);

        if (deleteAppResult.Success)
        {
            return deleteAppResult;
        }

        return new(ServiceOperationResultStatus.Success, "The application was deleted.");
    }

    private async Task<ServiceOperationResult<Application>> EnsureApplicationExistsAsync(string applicationNameOrId)
    {
        var getApplicationResult = await _graphService.GetApplicationByIdOrNameAsync(applicationNameOrId);

        if (getApplicationResult.Success && getApplicationResult.Data == null)
        {
            return new(ServiceOperationResultStatus.NotFound, $"The application Id/name provided, '{applicationNameOrId}', could not be found");
        }

        return getApplicationResult;
    }
}
