using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

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
        var status = await _appConfigurationService.AddAppScopeAsync(settings);

        switch (status)
        {
            case OperationResultStatus.Success: return 0;
            default: return 1;
        }
    }
}
