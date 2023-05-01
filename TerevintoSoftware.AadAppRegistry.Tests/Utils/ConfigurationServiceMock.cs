using Azure.Identity;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Services;

namespace TerevintoSoftware.AadAppRegistry.Tests.Utils;

internal class ConfigurationServiceMock : IConfigurationService
{
    private readonly OperatingMode _operatingMode;

    public string ConfigFilePath { get; } = "";

    public ConfigurationServiceMock(OperatingMode operatingMode)
    {
        _operatingMode = operatingMode;
    }

    public ClientSecretCredential GetAzureCredential()
    {
        var config = Load();

        return new(config.ClientCredentials.TenantId, config.ClientCredentials.ClientId, config.ClientCredentials.ClientSecret);
    }

    public AppRegistryConfiguration Load()
    {
        return new()
        {
            ClientCredentials = new()
            { 
                TenantId = Environment.GetEnvironmentVariable("APPREG_TESTS__TENANT_ID"),
                ClientId = Environment.GetEnvironmentVariable("APPREG_TESTS__CLIENT_ID"),
                ClientSecret = Environment.GetEnvironmentVariable("APPREG_TESTS__CLIENT_SECRET")
            },
            OperatingMode = _operatingMode,
            TenantName = Environment.GetEnvironmentVariable("APPREG_TESTS__TENANT_NAME")
        };
    }

    public void Save(AppRegistryConfiguration appRegistryConfiguration)
    {
    }
}
