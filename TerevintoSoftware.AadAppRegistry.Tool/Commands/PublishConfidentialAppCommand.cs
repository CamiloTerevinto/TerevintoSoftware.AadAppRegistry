using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;
using System.Text.Json;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

internal class PublishConfidentialAppCommand : AsyncCommand<PublishConfidentialCommandSettings>
{
    private readonly IAppRegistrationService _appRegistrationService;

    public PublishConfidentialAppCommand(IAppRegistrationService appRegistrationService)
    {
        _appRegistrationService = appRegistrationService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, PublishConfidentialCommandSettings settings)
    {
        var result = await _appRegistrationService.RegisterConfidentialApp(settings);

        switch (result.Status)
        {
            case OperationResultStatus.Success:
            {
                AnsiConsole.Write(new JsonText(JsonSerializer.Serialize(result.Data)));
                return 0;
            }
            case OperationResultStatus.AppRegistrationPreviouslyCreated: return 0;
            case OperationResultStatus.Failed: return 1;
            default: return 1;
        }
    }
}