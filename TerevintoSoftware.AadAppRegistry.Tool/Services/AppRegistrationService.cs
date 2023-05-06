using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Models;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Settings.Base;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IAppRegistrationService
{
    Task<ServiceOperationResult<ApiAppRegistrationResult>> RegisterApiApp(PublishApiCommandSettings settings);
    Task<ServiceOperationResult<ConfidentialAppRegistrationResult>> RegisterConfidentialAppAsync(PublishConfidentialCommandSettings settings);
    Task<ServiceOperationResult<NativeAppRegistrationResult>> RegisterNativeApp(PublishNativeCommandSettings settings);
    Task<ServiceOperationResult<SpaAppRegistrationResult>> RegisterSpaApp(PublishSpaCommandSettings settings);
    Task<ServiceOperationResult<WebAppRegistrationResult>> RegisterWebApp(PublishWebCommandSettings settings);
}

internal class AppRegistrationService : IAppRegistrationService
{
    private readonly IGraphClientService _graphService;
    private readonly IKeyVaultClientService _keyVaultClientService;
    private readonly AppRegistryConfiguration _appRegistryConfiguration;

    public AppRegistrationService(IGraphClientService graphService, IConfigurationService configurationService, IKeyVaultClientService keyVaultClientService)
    {
        _graphService = graphService;
        _keyVaultClientService = keyVaultClientService;
        _appRegistryConfiguration = configurationService.Load();
    }

    /// <summary>
    /// Registers an API (server) application in Azure AD. Optionally adds an API scope and sets the application ID URI.
    /// </summary>
    /// <param name="settings">The settings to use to create the application.</param>
    /// <returns>The result of the registration.</returns>
    public async Task<ServiceOperationResult<ApiAppRegistrationResult>> RegisterApiApp(PublishApiCommandSettings settings)
    {
        var shouldSkipCreationResult = await ShouldSkipCreationAsync(settings);

        if (!shouldSkipCreationResult.Success)
        {
            return new(shouldSkipCreationResult);
        }

        var applicationResult = await _graphService.CreateApplicationAsync(settings.ApplicationName, settings.SignInAudience);

        if (!applicationResult.Success)
        {
            return new(applicationResult);
        }

        var application = applicationResult.Data;
        var applicationUri = settings.ApplicationUri;
        var appRegistrationResult = new ApiAppRegistrationResult
        {
            Name = settings.ApplicationName,
            ClientId = Guid.Parse(application.AppId),
            ObjectId = Guid.Parse(application.Id)
        };

        if (string.IsNullOrEmpty(applicationUri) && settings.SetDefaultApplicationUri)
        {
            applicationUri = _appRegistryConfiguration.OperatingMode == OperatingMode.AzureB2C ?
                $"https://{_appRegistryConfiguration.TenantName}/{application.AppId}" :
                $"api://{application.AppId}";
        }

        if (!string.IsNullOrEmpty(applicationUri))
        {
            var result = await _graphService.SetApplicationIdUriAsync(application, applicationUri);

            if (!result.Success)
            {
                return new(appRegistrationResult, result);
            }

            appRegistrationResult.Uri = applicationUri;
        }

        if (settings.SetDefaultScope)
        {
            var result = await _graphService.AddApiScopeAsync(application, "access_as_user", "Access as user", 
                "Allows applications to perform requests to this API on the user's behalf.");

            if (!result.Success)
            {
                return new(appRegistrationResult, result);
            }

            appRegistrationResult.Scope = $"{appRegistrationResult.Uri}/access_as_user";
        }

        return appRegistrationResult;
    }

    /// <summary>
    /// Registers an SPA (Single Page Application) application in Azure AD.
    /// </summary>
    /// <param name="settings">The settings to use to create the application</param>
    /// <returns>The result of the registration.</returns>
    public async Task<ServiceOperationResult<SpaAppRegistrationResult>> RegisterSpaApp(PublishSpaCommandSettings settings)
    {
        var shouldSkipCreationResult = await ShouldSkipCreationAsync(settings);

        if (!shouldSkipCreationResult.Success)
        {
            return new(shouldSkipCreationResult);
        }

        var applicationResult = await _graphService.CreateApplicationAsync(settings.ApplicationName, settings.SignInAudience);

        if (!applicationResult.Success)
        {
            return new(applicationResult);
        }

        var application = applicationResult.Data;

        var appRegistrationResult = new SpaAppRegistrationResult
        {
            Name = settings.ApplicationName,
            ClientId = Guid.Parse(application.AppId),
            ObjectId = Guid.Parse(application.Id),
        };

        if (settings.RedirectUris != null && settings.RedirectUris.Length > 0)
        {
            var redirectUriResult = await _graphService.AddSpaRedirectUrisAsync(application, settings.RedirectUris);

            if (!redirectUriResult.Success)
            {
                return new(appRegistrationResult, redirectUriResult);
            }
        }

        return appRegistrationResult;
    }

    /// <summary>
    /// Registers a Native (mobile/desktop) application in Azure AD.
    /// </summary>
    /// <param name="settings">The settings to use to create the application</param>
    /// <returns>The result of the registration.</returns>
    public async Task<ServiceOperationResult<NativeAppRegistrationResult>> RegisterNativeApp(PublishNativeCommandSettings settings)
    {
        var shouldSkipCreationResult = await ShouldSkipCreationAsync(settings);

        if (!shouldSkipCreationResult.Success)
        {
            return new(shouldSkipCreationResult);
        }

        var applicationResult = await _graphService.CreateApplicationAsync(settings.ApplicationName, settings.SignInAudience);

        if (!applicationResult.Success)
        {
            return new(applicationResult);
        }

        var application = applicationResult.Data;
        var appRegistrationResult = new NativeAppRegistrationResult
        {
            Name = settings.ApplicationName,
            ClientId = Guid.Parse(application.AppId),
            ObjectId = Guid.Parse(application.Id),
        };

        if (settings.RedirectUris != null && settings.RedirectUris.Length > 0)
        {
            var addRedirectUriResult = await _graphService.AddNativeRedirectUrisAsync(application, settings.RedirectUris);

            if (!addRedirectUriResult.Success)
            {
                return new(appRegistrationResult, addRedirectUriResult);
            }
        }

        return appRegistrationResult;
    }

    /// <summary>
    /// Registers a Web (traditional server-side) application in Azure AD.
    /// </summary>
    /// <param name="settings">The settings to use to create the application</param>
    /// <returns>The result of the registration.</returns>
    public async Task<ServiceOperationResult<WebAppRegistrationResult>> RegisterWebApp(PublishWebCommandSettings settings)
    {
        var shouldSkipCreationResult = await ShouldSkipCreationAsync(settings);

        if (!shouldSkipCreationResult.Success)
        {
            return new(shouldSkipCreationResult);
        }

        var applicationResult = await _graphService.CreateApplicationAsync(settings.ApplicationName, settings.SignInAudience);

        if (!applicationResult.Success)
        {
            return new(applicationResult);
        }

        var application = applicationResult.Data;
        var appRegistrationResult = new WebAppRegistrationResult
        {
            Name = settings.ApplicationName,
            ClientId = Guid.Parse(application.AppId),
            ObjectId = Guid.Parse(application.Id),
        };

        if (settings.RedirectUris != null && settings.RedirectUris.Length > 0)
        {
            var addRedirectUriResult = await _graphService.AddWebRedirectUrisAsync(application, settings.RedirectUris);

            if (!addRedirectUriResult.Success)
            {
                return new(appRegistrationResult, addRedirectUriResult);
            }
        }

        return appRegistrationResult;
    }

    /// <summary>
    /// Registers a Confidential (Client Credentials) application in Azure AD. Optionally creates a client secret.
    /// </summary>
    /// <param name="settings">The settings to use to create the application</param>
    /// <returns>The result of the registration.</returns>
    public async Task<ServiceOperationResult<ConfidentialAppRegistrationResult>> RegisterConfidentialAppAsync(PublishConfidentialCommandSettings settings)
    {
        var shouldSkipCreationResult = await ShouldSkipCreationAsync(settings);
        
        if (!shouldSkipCreationResult.Success)
        {
            return new(shouldSkipCreationResult);
        }

        var applicationResult = await _graphService.CreateApplicationAsync(settings.ApplicationName, settings.SignInAudience);

        if (!applicationResult.Success)
        {
            return new(applicationResult);
        }

        var application = applicationResult.Data;
        var appRegistrationResult = new ConfidentialAppRegistrationResult
        {
            Name = settings.ApplicationName,
            ClientId = Guid.Parse(application.AppId),
            ObjectId = Guid.Parse(application.Id),
            Secret = ""
        };

        if (settings.CreateClientSecret)
        {
            var expiration = settings.ClientSecretExpirationDays == 0 ? null : (DateTimeOffset?)DateTimeOffset.Now.AddDays(settings.ClientSecretExpirationDays);

            var secretCreationResult = await _graphService.AddClientSecretAsync(application, expiration);

            if (!secretCreationResult.Success)
            {
                return new(appRegistrationResult, secretCreationResult);
            }

            appRegistrationResult.Secret = secretCreationResult.Data;
        }

        var keyVaultResult = await PushSecretToKeyVaultAsync(settings, appRegistrationResult);
        
        if (!keyVaultResult.Success)
        {
            return new(appRegistrationResult, keyVaultResult);
        }

        return appRegistrationResult;
    }

    /// <summary>
    /// Determines whether creation of the App Registration should be skipped
    /// </summary>
    /// <param name="publishCommandSettings">The base settings for the application</param>
    private async Task<ServiceOperationResult> ShouldSkipCreationAsync(PublishCommandBaseSettings publishCommandSettings)
    {
        if (publishCommandSettings.DisableDuplicateCheck)
        {
            return ServiceOperationResultStatus.Success;
        }

        var getApplicationResult = await _graphService.GetApplicationByDisplayNameAsync(publishCommandSettings.ApplicationName);

        if (!getApplicationResult.Success)
        {
            return new(getApplicationResult);
        }

        if (getApplicationResult.Data != null)
        {
            return ServiceOperationResultStatus.AppRegistrationPreviouslyCreated;
        }

        return ServiceOperationResultStatus.Success;
    }

    private async Task<ServiceOperationResult> PushSecretToKeyVaultAsync(PublishConfidentialCommandSettings settings, ConfidentialAppRegistrationResult result)
    {
        if (result.Secret == null || string.IsNullOrEmpty(settings.KeyVaultUri))
        {
            return ServiceOperationResultStatus.Success;
        }

        try
        {
            var expirationDate = DateTime.UtcNow.Date.AddDays(settings.ClientSecretExpirationDays);
            var secretUri = await _keyVaultClientService.PushClientSecretAsync(new Uri(settings.KeyVaultUri), settings.SecretName, result.Secret, expirationDate);
            result.Secret = secretUri.ToString();

            return ServiceOperationResultStatus.Success;
        }
        catch (Exception ex)
        {
            return new(ServiceOperationResultStatus.Failed, $"While the application was registered, pushing the secret to Key Vault failed: {ex.Message}.");
        }
    }
}
