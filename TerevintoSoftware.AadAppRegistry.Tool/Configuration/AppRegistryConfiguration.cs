using System.Diagnostics.CodeAnalysis;

namespace TerevintoSoftware.AadAppRegistry.Tool.Configuration;

[ExcludeFromCodeCoverage]
public class AppRegistryConfiguration
{
    public ClientCredentialsOptions ClientCredentials { get; set; }
    public OperatingMode OperatingMode { get; set; }
    public string TenantName { get; set; }

    public AppRegistryConfiguration()
    {
        ClientCredentials = new ClientCredentialsOptions();
        OperatingMode = OperatingMode.AzureActiveDirectory;
    }
}
