using TerevintoSoftware.AadAppRegistry.Tests.Utils;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tests.IntegrationTests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class AppRegistrationsConfidentialTest
{
    private IAppRegistrationService _registrationService;

    [SetUp]
    public void Setup()
    {
        _registrationService = ContainerBuilder.BuildServicesFor<IAppRegistrationService>();
    }

    [Test]
    public async Task RegisterConfidentialAppTest()
    {
        var appName = Helpers.GetAppName();
        var command = new PublishConfidentialCommandSettings
        {
            ApplicationName = appName,
            DisableDuplicateCheck = false,
            SignInAudience = SignInAudienceType.AzureADandPersonalMicrosoftAccount,
            CreateClientSecret = true
        };

        var result = await _registrationService.RegisterConfidentialAppAsync(command);

        Assert.That(result.Status, Is.EqualTo(ServiceOperationResultStatus.Success));
        TestContext.Out.WriteLine($"Application registered with id {result.Data.ClientId}");
        TestContext.Out.WriteLine($"Application registered with secret {result.Data.Secret}");

        await Helpers.DeleteAppRegistrationAsync(appName, result.Data.ClientId);
    }

    [Test]
    public async Task KeyVault_RegisterConfidentialAppTest()
    {
        var appName = Helpers.GetAppName();
        var command = new PublishConfidentialCommandSettings
        {
            ApplicationName = appName,
            DisableDuplicateCheck = false,
            SignInAudience = SignInAudienceType.AzureADandPersonalMicrosoftAccount,
            CreateClientSecret = true,
            KeyVaultUri = Environment.GetEnvironmentVariable("APPREG_TESTS__KEYVAULT_URI"),
            SecretName = appName.TrimStart('_')
        };

        command.Validate();

        var result = await _registrationService.RegisterConfidentialAppAsync(command);

        Assert.That(result.Status, Is.EqualTo(ServiceOperationResultStatus.Success), result.Message);
        TestContext.Out.WriteLine($"Application registered with id {result.Data.ClientId}");
        TestContext.Out.WriteLine($"Application registered with secret {result.Data.Secret}");

        await Helpers.DeleteAppRegistrationAsync(appName, result.Data.ClientId);
    }
}
