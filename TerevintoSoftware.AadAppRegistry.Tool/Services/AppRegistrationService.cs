using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Models;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IAppRegistrationService
{
    Task<OperationResult<ApiAppRegistrationResult>> RegisterApiApp(PublishApiCommandSettings settings);
}

internal class AppRegistrationService : IAppRegistrationService
{
    private readonly IAppRegistrationsGraphService _graphService;
    private readonly AppRegistryConfiguration _appRegistryConfiguration;

    public AppRegistrationService(IAppRegistrationsGraphService graphService, IConfigurationService configurationService)
    {
        _graphService = graphService;
        _appRegistryConfiguration = configurationService.Load();
    }

    public async Task<OperationResult<ApiAppRegistrationResult>> RegisterApiApp(PublishApiCommandSettings settings)
    {
        if (await ShouldSkipCreationAsync(settings))
        {
            return OperationResultStatus.AppRegistrationPreviouslyCreated;
        }

        var application = await _graphService.CreateApplicationAsync(settings.ApplicationName, settings.SignInAudience);

        var applicationUri = settings.ApplicationUri;

        if (string.IsNullOrEmpty(applicationUri) && settings.SetDefaultApplicationUri)
        {
            applicationUri = _appRegistryConfiguration.OperatingMode == OperatingMode.AzureB2C ?
                $"https://{_appRegistryConfiguration.TenantName}.onmicrosoft.com/{application.AppId}" :
                $"api://{application.AppId}";
        }

        if (!string.IsNullOrEmpty(applicationUri))
        {
            await _graphService.SetApplicationIdUriAsync(application, applicationUri);
        }

        if (settings.SetDefaultScope)
        {
            await _graphService.AddApiScopeAsync(application, "access_as_user", "Access as user", "Allows applications to perform requests to this client on the user's behalf.");
        }

        var finalUri = applicationUri ?? "";
        var scope = finalUri == "" ? "" : $"{finalUri}/access_as_user";

        return new ApiAppRegistrationResult
        {
            Name = settings.ApplicationName,
            ClientId = Guid.Parse(application.AppId),
            ObjectId = Guid.Parse(application.Id),
            Uri = finalUri,
            Scope = scope
        };
    }

    /// <summary>
    /// Determines whether creation of the App Registration should be skipped
    /// </summary>
    /// <param name="publishCommandSettings">The base settings for the application</param>
    /// <returns>True if creation should be skipped; false otherwise.</returns>
    private async Task<bool> ShouldSkipCreationAsync(PublishCommandSettings publishCommandSettings)
    {
        if (publishCommandSettings.DisableDuplicateCheck)
        {
            return false;
        }

        var app = await _graphService.GetApplicationByDisplayNameAsync(publishCommandSettings.ApplicationName);

        return app != null;
    }
}
