using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

internal class ConfigureCredentialsCommand : Command<ClientCredentialsSettings>
{
    private readonly IConfigurationService _configurationService;

    public ConfigureCredentialsCommand(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] ClientCredentialsSettings settings)
    {
        var configuration = _configurationService.Load();

        configuration.ClientCredentials = new ClientCredentialsOptions
        {
            ClientId = settings.ClientId,
            ClientSecret = settings.ClientSecret,
            TenantId = settings.TenantId,
        };

        _configurationService.Save(configuration);

        AnsiConsole.MarkupLine($"[bold green]Success:[/] credentials stored under {_configurationService.ConfigFilePath}");

        return 0;
    }
}
