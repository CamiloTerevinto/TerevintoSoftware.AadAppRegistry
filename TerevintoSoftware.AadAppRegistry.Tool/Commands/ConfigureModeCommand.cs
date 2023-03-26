using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Services;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

public class ConfigureModeCommand : Command<ConfigureModeSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] ConfigureModeSettings settings)
    {
        var config = new ConfigurationService();
        config.Load();

        AppRegistryConfiguration.Instance.OperatingMode = settings.UseB2CMode ? OperatingMode.AzureB2C : OperatingMode.AzureActiveDirectory;

        config.Save();

        AnsiConsole.MarkupLine($"[bold green]Success:[/] operating mode changed to {AppRegistryConfiguration.Instance.OperatingMode}");

        return 0;
    }
}

public class ConfigureModeSettings : CommandSettings
{
    [CommandOption("--use-b2c")]
    [Description("Whether to use the AAD (false) or B2C (true) mode")]
    public bool UseB2CMode { get; init; }
}