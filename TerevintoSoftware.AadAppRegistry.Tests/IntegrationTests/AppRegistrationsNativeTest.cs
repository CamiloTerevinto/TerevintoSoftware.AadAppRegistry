using TerevintoSoftware.AadAppRegistry.Tests.Utils;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tests.IntegrationTests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class AppRegistrationsNativeTest
{
    private IAppRegistrationService _registrationService;

    [SetUp]
    public void Setup()
    {
        _registrationService = ContainerBuilder.BuildServicesFor<IAppRegistrationService>();
    }

    [Test]
    public async Task RegisterNativeAppTest()
    {
        var appName = Helpers.GetAppName();
        var command = new PublishNativeCommandSettings
        {
            ApplicationName = appName,
            DisableDuplicateCheck = false,
            RedirectUris = new[] { "mytestapp://signin-oidc" },
            SignInAudience = SignInAudienceType.AzureADandPersonalMicrosoftAccount
        };

        var result = await _registrationService.RegisterNativeApp(command);

        Assert.That(result.Status, Is.EqualTo(ServiceOperationResultStatus.Success));
        TestContext.Out.WriteLine($"Application registered with id {result.Data.ClientId}");

        await Helpers.DeleteAppRegistrationAsync(appName, result.Data.ClientId);
    }
}
