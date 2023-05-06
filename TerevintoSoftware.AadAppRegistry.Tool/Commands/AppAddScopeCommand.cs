using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

[ExcludeFromCodeCoverage]
internal class AppAddScopeCommand : AsyncCommand<ConfigureAppAddScopeSettings>
{
    private readonly IAppConfigurationService _appConfigurationService;

    public AppAddScopeCommand(IAppConfigurationService appRegistrationService)
    {
        _appConfigurationService = appRegistrationService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ConfigureAppAddScopeSettings settings)
    {
        return await _appConfigurationService.AddAppScopeAsync(settings).ExecuteOperationAsync();
    }
}
