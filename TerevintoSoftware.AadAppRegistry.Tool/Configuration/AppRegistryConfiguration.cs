namespace TerevintoSoftware.AadAppRegistry.Tool.Configuration;

public class AppRegistryConfiguration
{
    public ClientCredentialsOptions ClientCredentials { get; set; }
    public OperatingMode OperatingMode { get; set; }

    public static AppRegistryConfiguration Instance { get; internal set; }

    public AppRegistryConfiguration()
    {
        ClientCredentials = new ClientCredentialsOptions();
        OperatingMode = OperatingMode.AzureActiveDirectory;
    }
}
