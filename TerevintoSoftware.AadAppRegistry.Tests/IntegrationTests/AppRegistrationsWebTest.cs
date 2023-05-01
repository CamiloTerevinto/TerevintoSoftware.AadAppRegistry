using TerevintoSoftware.AadAppRegistry.Tests.Utils;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tests.IntegrationTests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class AppRegistrationsWebTest
{
    private IAppRegistrationService _registrationService;

    [SetUp]
    public void Setup()
    {
        _registrationService = ContainerBuilder.BuildServicesFor<IAppRegistrationService>();
    }

    [Test]
    public async Task RegisterWebAppTest()
    {
        var appName = Helpers.GetAppName();
        var command = new PublishWebCommandSettings
        {
            ApplicationName = appName,
            DisableDuplicateCheck = false,
            RedirectUris = new[] { "https://localhost:5001/signin-oidc" },
            SignInAudience = SignInAudienceType.AzureADandPersonalMicrosoftAccount
        };

        var result = await _registrationService.RegisterWebApp(command);

        Assert.That(result.Status, Is.EqualTo(OperationResultStatus.Success));
        TestContext.Out.WriteLine($"Application registered with id {result.Data.ClientId}");

        await Helpers.DeleteAppRegistrationAsync(appName, result.Data.ClientId);
    }
}
