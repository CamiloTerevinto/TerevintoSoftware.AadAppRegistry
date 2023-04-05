using Spectre.Console.Cli;
using TerevintoSoftware.AadAppRegistry.Tool.Services;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

internal class ValidateCommand : AsyncCommand
{
    private readonly IAppRegistrationsGraphService _appRegistrationsGraphService;

    public ValidateCommand(IAppRegistrationsGraphService appRegistrationsGraphService)
    {
        _appRegistrationsGraphService = appRegistrationsGraphService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var success = await _appRegistrationsGraphService.ValidateConnection();

        return success ? 0 : 1;
    }
}
