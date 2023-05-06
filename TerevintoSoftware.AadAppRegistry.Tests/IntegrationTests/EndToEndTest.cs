using TerevintoSoftware.AadAppRegistry.Tests.Utils;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tests.IntegrationTests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class EndToEndTest
{
    [Test]
    public async Task AAD_EndToEnd()
    {
        var registrationService = ContainerBuilder.BuildServicesFor<IAppRegistrationService>(OperatingMode.AzureActiveDirectory);
        var configurationService = ContainerBuilder.BuildServicesFor<IAppConfigurationService>(OperatingMode.AzureActiveDirectory);

        var apiAppName = Helpers.GetAppName();
        var apiCommand = new PublishApiCommandSettings
        {
            ApplicationName = apiAppName,
            DisableDuplicateCheck = false,
            SignInAudience = SignInAudienceType.AzureADandPersonalMicrosoftAccount,
            SetDefaultApplicationUri = true,
            SetDefaultScope = true,
        };

        var apiRegistrationResult = await registrationService.RegisterApiApp(apiCommand);

        Assert.That(apiRegistrationResult.Status, Is.EqualTo(ServiceOperationResultStatus.Success));
        TestContext.Out.WriteLine($"API Application registered with id {apiRegistrationResult.Data.ClientId}");
        TestContext.Out.WriteLine($"API Application registered with application uri {apiRegistrationResult.Data.Uri}");
        TestContext.Out.WriteLine($"API Application registered with scope {apiRegistrationResult.Data.Scope}");

        var spaAppName = Helpers.GetAppName();
        var spaCommand = new PublishSpaCommandSettings
        {
            ApplicationName = spaAppName,
            DisableDuplicateCheck = false,
            RedirectUris = new[] { "https://localhost:5001/signin-oidc" },
            SignInAudience = SignInAudienceType.AzureADandPersonalMicrosoftAccount
        };

        var spaRegistrationResult = await registrationService.RegisterSpaApp(spaCommand);
        TestContext.Out.WriteLine($"SPA Application registered with id {spaRegistrationResult.Data.ClientId}");

        ServiceOperationResult addScopeResult;
        int triesLeft = 3;

        do
        {
            // This is retried a few times as the applications and scope are not immediately available in the Graph API to be read.
            await Task.Delay(10_000);
            triesLeft--;
            addScopeResult = await configurationService.AddAppScopeAsync(new()
            {
                ApiAppId = apiRegistrationResult.Data.ClientId.ToString(),
                ApplicationName = spaRegistrationResult.Data.ClientId.ToString(),
                ScopeName = "access_as_user"
            });
        }
        while (addScopeResult.Status == ServiceOperationResultStatus.NotFound && triesLeft >= 0);

        await Helpers.DeleteAppRegistrationAsync(apiAppName, apiRegistrationResult.Data.ClientId);
        await Helpers.DeleteAppRegistrationAsync(spaAppName, spaRegistrationResult.Data.ClientId);

        Assert.That(addScopeResult.Status, Is.EqualTo(ServiceOperationResultStatus.Success));
        TestContext.Out.WriteLine($"Scope access_as_user added to SPA Application {spaRegistrationResult.Data.ClientId}");
    }

    [Test]
    public async Task B2C_EndToEnd()
    {
        var registrationService = ContainerBuilder.BuildServicesFor<IAppRegistrationService>(OperatingMode.AzureB2C);
        var configurationService = ContainerBuilder.BuildServicesFor<IAppConfigurationService>(OperatingMode.AzureB2C);

        var apiAppName = Helpers.GetAppName();
        var apiCommand = new PublishApiCommandSettings
        {
            ApplicationName = apiAppName,
            DisableDuplicateCheck = false,
            SignInAudience = SignInAudienceType.AzureADandPersonalMicrosoftAccount,
            SetDefaultApplicationUri = true,
            SetDefaultScope = true,
        };

        var apiRegistrationResult = await registrationService.RegisterApiApp(apiCommand);

        Assert.That(apiRegistrationResult.Status, Is.EqualTo(ServiceOperationResultStatus.Success));
        TestContext.Out.WriteLine($"API Application registered with id {apiRegistrationResult.Data.ClientId}");
        TestContext.Out.WriteLine($"API Application registered with application uri {apiRegistrationResult.Data.Uri}");
        TestContext.Out.WriteLine($"API Application registered with scope {apiRegistrationResult.Data.Scope}");

        var spaAppName = Helpers.GetAppName();
        var spaCommand = new PublishSpaCommandSettings
        {
            ApplicationName = spaAppName,
            DisableDuplicateCheck = false,
            RedirectUris = new[] { "https://localhost:5001/signin-oidc" },
            SignInAudience = SignInAudienceType.AzureADandPersonalMicrosoftAccount
        };

        var spaRegistrationResult = await registrationService.RegisterSpaApp(spaCommand);
        TestContext.Out.WriteLine($"SPA Application registered with id {spaRegistrationResult.Data.ClientId}");

        ServiceOperationResult addScopeResult;
        int triesLeft = 3;

        do
        {
            // This is retried a few times as the applications and scope are not immediately available in the Graph API to be read.
            await Task.Delay(10_000);
            triesLeft--;
            addScopeResult = await configurationService.AddAppScopeAsync(new()
            {
                ApiAppId = apiRegistrationResult.Data.ClientId.ToString(),
                ApplicationName = spaRegistrationResult.Data.ClientId.ToString(),
                ScopeName = "access_as_user"
            });
        }
        while (addScopeResult.Status == ServiceOperationResultStatus.NotFound && triesLeft >= 0);

        await Helpers.DeleteAppRegistrationAsync(apiAppName, apiRegistrationResult.Data.ClientId);
        await Helpers.DeleteAppRegistrationAsync(spaAppName, spaRegistrationResult.Data.ClientId);

        Assert.That(addScopeResult.Status, Is.EqualTo(ServiceOperationResultStatus.Success));
        TestContext.Out.WriteLine($"Scope access_as_user added to SPA Application {spaRegistrationResult.Data.ClientId}");
    }
}
