using TerevintoSoftware.AadAppRegistry.Tests.Utils;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tests.IntegrationTests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class AppRegistrationsApiTest
{
    [Test]
    public async Task AAD_RegisterApiAppTest()
    {
        var registrationService = ContainerBuilder.BuildServicesFor<IAppRegistrationService>(OperatingMode.AzureActiveDirectory);
        var appName = Helpers.GetAppName();
        var command = new PublishApiCommandSettings
        {
            ApplicationName = appName,
            DisableDuplicateCheck = false,
            SignInAudience = SignInAudienceType.AzureADandPersonalMicrosoftAccount,
            SetDefaultApplicationUri = true,
            SetDefaultScope = true,
        };

        var result = await registrationService.RegisterApiApp(command);

        Assert.That(result.Status, Is.EqualTo(ServiceOperationResultStatus.Success));
        TestContext.Out.WriteLine($"Application registered with id {result.Data.ClientId}");
        TestContext.Out.WriteLine($"Application registered with application uri {result.Data.Uri}");
        TestContext.Out.WriteLine($"Application registered with scope {result.Data.Scope}");

        await Helpers.DeleteAppRegistrationAsync(appName, result.Data.ClientId);
    }

    [Test]
    public async Task B2C_RegisterApiAppTest()
    {
        var registrationService = ContainerBuilder.BuildServicesFor<IAppRegistrationService>(OperatingMode.AzureB2C);

        var appName = Helpers.GetAppName();
        var command = new PublishApiCommandSettings
        {
            ApplicationName = appName,
            DisableDuplicateCheck = false,
            SignInAudience = SignInAudienceType.AzureADandPersonalMicrosoftAccount,
            SetDefaultApplicationUri = true,
            SetDefaultScope = true,
        };

        var result = await registrationService.RegisterApiApp(command);

        Assert.That(result.Status, Is.EqualTo(ServiceOperationResultStatus.Success));
        TestContext.Out.WriteLine($"Application registered with id {result.Data.ClientId}");
        TestContext.Out.WriteLine($"Application registered with application uri {result.Data.Uri}");
        TestContext.Out.WriteLine($"Application registered with scope {result.Data.Scope}");

        await Helpers.DeleteAppRegistrationAsync(appName, result.Data.ClientId);

        Assert.That(result.Data.Uri, Does.StartWith("https://"));
    }
}
