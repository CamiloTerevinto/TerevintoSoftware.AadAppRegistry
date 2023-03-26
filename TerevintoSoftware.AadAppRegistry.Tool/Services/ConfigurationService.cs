using System.Text.Json;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal class ConfigurationService
{
    private readonly string _configFilePath;

    public ConfigurationService()
    {
        _configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "AadAppRegistryConfig.json");
    }

    public void Load()
    {
        if (File.Exists(_configFilePath))
        {
            AppRegistryConfiguration.Instance = JsonSerializer.Deserialize<AppRegistryConfiguration>(File.ReadAllText(_configFilePath));
        }

        AppRegistryConfiguration.Instance = new();
    }

    public void Save()
    {
        File.WriteAllText(_configFilePath, JsonSerializer.Serialize(AppRegistryConfiguration.Instance));
    }
}
