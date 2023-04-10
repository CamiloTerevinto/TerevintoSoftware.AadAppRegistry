using Spectre.Console.Cli;
using TerevintoSoftware.AadAppRegistry.Tool.Services;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

internal class ValidateCommand : AsyncCommand
{
    private readonly IGraphClientService _appRegistrationsGraphService;

    public ValidateCommand(IGraphClientService appRegistrationsGraphService)
    {
        _appRegistrationsGraphService = appRegistrationsGraphService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        await _appRegistrationsGraphService.ValidateConnectionAsync();

        return 0;
    }
}
