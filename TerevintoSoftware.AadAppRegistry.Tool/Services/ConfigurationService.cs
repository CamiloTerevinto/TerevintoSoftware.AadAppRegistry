using System.Text.Json;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IConfigurationService
{
    AppRegistryConfiguration Load();
    void Save(AppRegistryConfiguration appRegistryConfiguration);
}

internal class ConfigurationService : IConfigurationService
{
    private readonly string _configFilePath;

    public ConfigurationService()
    {
        _configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "AadAppRegistryConfig.json");
    }

    public AppRegistryConfiguration Load()
    {
        if (File.Exists(_configFilePath))
        {
            return JsonSerializer.Deserialize<AppRegistryConfiguration>(File.ReadAllText(_configFilePath));
        }

        return new();
    }

    public void Save(AppRegistryConfiguration appRegistryConfiguration)
    {
        File.WriteAllText(_configFilePath, JsonSerializer.Serialize(appRegistryConfiguration));
    }
}
