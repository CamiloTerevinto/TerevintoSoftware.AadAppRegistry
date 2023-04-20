using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

internal class ConfigureGeneralSettingsCommand : Command<GeneralConfigurationCommandSettings>
{
    private readonly IConfigurationService _configurationService;

    public ConfigureGeneralSettingsCommand(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] GeneralConfigurationCommandSettings settings)
    {
        var configuration = _configurationService.Load();

        configuration.OperatingMode = settings.UseB2CMode ? OperatingMode.AzureB2C : OperatingMode.AzureActiveDirectory;
        configuration.TenantName = !string.IsNullOrEmpty(settings.TenantName) ? settings.TenantName : configuration.TenantName;

        if (configuration.OperatingMode == OperatingMode.AzureB2C && string.IsNullOrEmpty(configuration.TenantName))
        {
            AnsiConsole.MarkupLine($"[bold red]Error:[/] tenant name is required when using Azure B2C mode.");
            return 1;
        }

        _configurationService.Save(configuration);

        AnsiConsole.MarkupLine($"[bold green]Success:[/] configuration updated.");

        return 0;
    }
}
