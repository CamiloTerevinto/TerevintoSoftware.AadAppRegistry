using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

[ExcludeFromCodeCoverage]
internal class PublishApiAppCommand : AsyncCommand<PublishApiCommandSettings>
{
    private readonly IAppRegistrationService _appRegistrationService;

    public PublishApiAppCommand(IAppRegistrationService appRegistrationService)
    {
        _appRegistrationService = appRegistrationService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, PublishApiCommandSettings settings)
    {
        return await _appRegistrationService.RegisterApiApp(settings).ExecuteOperationAsync();
    }
}
