using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tests.Utils;

internal static class Helpers
{
    private static readonly IAppConfigurationService _configurationService = ContainerBuilder.BuildServicesFor<IAppConfigurationService>();

    internal static async Task DeleteAppRegistrationAsync(string appName, Guid clientId)
    {
        var deleteResult = await _configurationService.DeleteAppAsync(new()
        {
            ApplicationName = appName,
            SuppressConfirmation = false
        });

        Assert.That(deleteResult.Status, Is.EqualTo(ServiceOperationResultStatus.Success));
        TestContext.Out.WriteLine($"Application with id {clientId} deleted");
    }

    internal static string GetAppName()
    {
        return $"_{Guid.NewGuid().ToString()[0..10]}";
    }
}
