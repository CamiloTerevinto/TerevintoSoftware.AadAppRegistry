using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

internal class ConfigureModeCommand : Command<ConfigureModeSettings>
{
    private readonly IConfigurationService _configurationService;

    public ConfigureModeCommand(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] ConfigureModeSettings settings)
    {
        var configuration = _configurationService.Load();

        configuration.OperatingMode = settings.UseB2CMode ? OperatingMode.AzureB2C : OperatingMode.AzureActiveDirectory;

        _configurationService.Save(configuration);

        AnsiConsole.MarkupLine($"[bold green]Success:[/] operating mode changed to {configuration.OperatingMode}");

        return 0;
    }
}
