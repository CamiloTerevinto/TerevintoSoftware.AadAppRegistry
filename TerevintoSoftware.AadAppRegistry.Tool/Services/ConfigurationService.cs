using System.Text.Json;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IConfigurationService
{
    string ConfigFilePath { get; }

    AppRegistryConfiguration Load();
    void Save(AppRegistryConfiguration appRegistryConfiguration);
}

internal class ConfigurationService : IConfigurationService
{
    public string ConfigFilePath { get; }

    public ConfigurationService()
    {
        ConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "appreg.json");
    }

    public AppRegistryConfiguration Load()
    {
        if (File.Exists(ConfigFilePath))
        {
            return JsonSerializer.Deserialize<AppRegistryConfiguration>(File.ReadAllText(ConfigFilePath));
        }

        return new();
    }

    public void Save(AppRegistryConfiguration appRegistryConfiguration)
    {
        File.WriteAllText(ConfigFilePath, JsonSerializer.Serialize(appRegistryConfiguration));
    }
}
