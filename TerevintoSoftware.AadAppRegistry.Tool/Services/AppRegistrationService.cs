using Spectre.Console;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Models;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Settings.Base;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IAppRegistrationService
{
    Task<OperationResult<ApiAppRegistrationResult>> RegisterApiApp(PublishApiCommandSettings settings);
    Task<OperationResult<ConfidentialAppRegistrationResult>> RegisterConfidentialAppAsync(PublishConfidentialCommandSettings settings);
    Task<OperationResult<NativeAppRegistrationResult>> RegisterNativeApp(PublishNativeCommandSettings settings);
    Task<OperationResult<SpaAppRegistrationResult>> RegisterSpaApp(PublishSpaCommandSettings settings);
    Task<OperationResult<WebAppRegistrationResult>> RegisterWebApp(PublishWebCommandSettings settings);
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
                $"https://{_appRegistryConfiguration.TenantName}/{application.AppId}" :
                $"api://{application.AppId}";
        }

        if (!string.IsNullOrEmpty(applicationUri))
        {
            await _graphService.SetApplicationIdUriAsync(application, applicationUri);
        }

        if (settings.SetDefaultScope)
        {
            await _graphService.AddApiScopeAsync(application, "access_as_user", "Access as user", 
                "Allows applications to perform requests to this client on the user's behalf.");
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

    public async Task<OperationResult<SpaAppRegistrationResult>> RegisterSpaApp(PublishSpaCommandSettings settings)
    {
        if (await ShouldSkipCreationAsync(settings))
        {
            return OperationResultStatus.AppRegistrationPreviouslyCreated;
        }
        
        var application = await _graphService.CreateApplicationAsync(settings.ApplicationName, settings.SignInAudience);

        if (settings.RedirectUris != null && settings.RedirectUris.Length > 0)
        {
            await _graphService.AddSpaRedirectUrisAsync(application, settings.RedirectUris);
        }

        return new SpaAppRegistrationResult
        {
            Name = settings.ApplicationName,
            ClientId = Guid.Parse(application.AppId),
            ObjectId = Guid.Parse(application.Id),
        };
    }

    public async Task<OperationResult<NativeAppRegistrationResult>> RegisterNativeApp(PublishNativeCommandSettings settings)
    {
        if (await ShouldSkipCreationAsync(settings))
        {
            return OperationResultStatus.AppRegistrationPreviouslyCreated;
        }

        var application = await _graphService.CreateApplicationAsync(settings.ApplicationName, settings.SignInAudience);

        if (settings.RedirectUris != null && settings.RedirectUris.Length > 0)
        {
            await _graphService.AddNativeRedirectUrisAsync(application, settings.RedirectUris);
        }

        return new NativeAppRegistrationResult
        {
            Name = settings.ApplicationName,
            ClientId = Guid.Parse(application.AppId),
            ObjectId = Guid.Parse(application.Id),
        };
    }

    public async Task<OperationResult<WebAppRegistrationResult>> RegisterWebApp(PublishWebCommandSettings settings)
    {
        if (await ShouldSkipCreationAsync(settings))
        {
            return OperationResultStatus.AppRegistrationPreviouslyCreated;
        }
        
        var application = await _graphService.CreateApplicationAsync(settings.ApplicationName, settings.SignInAudience);
        
        if (settings.RedirectUris != null && settings.RedirectUris.Length > 0)
        {
            await _graphService.AddWebRedirectUrisAsync(application, settings.RedirectUris);
        }

        return new WebAppRegistrationResult
        {
            Name = settings.ApplicationName,
            ClientId = Guid.Parse(application.AppId),
            ObjectId = Guid.Parse(application.Id),
        };
    }

    public async Task<OperationResult<ConfidentialAppRegistrationResult>> RegisterConfidentialAppAsync(PublishConfidentialCommandSettings settings)
    {
        if (await ShouldSkipCreationAsync(settings))
        {
            return OperationResultStatus.AppRegistrationPreviouslyCreated;
        }
        
        var application = await _graphService.CreateApplicationAsync(settings.ApplicationName, settings.SignInAudience);
        var secret = "";

        if (settings.CreateClientSecret)
        {
            var expiration = settings.ClientSecretExpirationDays == 0 ? null : (DateTimeOffset?)DateTimeOffset.Now.AddDays(settings.ClientSecretExpirationDays);

            secret = await _graphService.AddClientSecretAsync(application, expiration);
        }

        var result = new ConfidentialAppRegistrationResult
        {
            Name = settings.ApplicationName,
            ClientId = Guid.Parse(application.AppId),
            ObjectId = Guid.Parse(application.Id),
            Secret = secret
        };

        var keyVaultStatus = await PushSecretToKeyVaultAsync(settings, result);
        
        return new OperationResult<ConfidentialAppRegistrationResult>(result, keyVaultStatus);
    }

    /// <summary>
    /// Determines whether creation of the App Registration should be skipped
    /// </summary>
    /// <param name="publishCommandSettings">The base settings for the application</param>
    /// <returns>True if creation should be skipped; false otherwise.</returns>
    private async Task<bool> ShouldSkipCreationAsync(PublishCommandBaseSettings publishCommandSettings)
    {
        if (publishCommandSettings.DisableDuplicateCheck)
        {
            return false;
        }

        var app = await _graphService.GetApplicationByDisplayNameAsync(publishCommandSettings.ApplicationName);

        return app != null;
    }

    private async Task<OperationResultStatus> PushSecretToKeyVaultAsync(PublishConfidentialCommandSettings settings, ConfidentialAppRegistrationResult result)
    {
        if (result.Secret == null || string.IsNullOrEmpty(settings.KeyVaultUri))
        {
            return OperationResultStatus.Success;
        }

        try
        {
            var expirationDate = DateTime.UtcNow.Date.AddDays(settings.ClientSecretExpirationDays);
            var secretUri = await _keyVaultClientService.PushClientSecretAsync(new Uri(settings.KeyVaultUri), settings.SecretName, result.Secret, expirationDate);
            result.Secret = secretUri.ToString();

            return OperationResultStatus.Success;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Error:[/] while the application was registered, pushing the secret to Key Vault failed: {ex.Message}.");

            return OperationResultStatus.Failed;
        }
    }
}
