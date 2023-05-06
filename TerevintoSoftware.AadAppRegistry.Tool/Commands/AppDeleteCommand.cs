using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

[ExcludeFromCodeCoverage]
internal class AppDeleteCommand : AsyncCommand<DeleteAppSettings>
{
    private readonly IAppConfigurationService _appConfigurationService;

    public AppDeleteCommand(IAppConfigurationService appRegistrationService)
    {
        _appConfigurationService = appRegistrationService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, DeleteAppSettings settings)
    {
        if (!settings.SuppressConfirmation)
        {
            var confirmed = AnsiConsole.Confirm("Are you sure you want to delete the app registration?");
            if (!confirmed)
            {
                return 0;
            }
        }

        return await _appConfigurationService.DeleteAppAsync(settings).ExecuteOperationAsync();
    }
}
