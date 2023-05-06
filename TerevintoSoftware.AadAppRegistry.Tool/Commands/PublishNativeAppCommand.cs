using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

[ExcludeFromCodeCoverage]
internal class PublishNativeAppCommand : AsyncCommand<PublishSpaCommandSettings>
{
    private readonly IAppRegistrationService _appRegistrationService;

    public PublishNativeAppCommand(IAppRegistrationService appRegistrationService)
    {
        _appRegistrationService = appRegistrationService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, PublishSpaCommandSettings settings)
    {
        return await _appRegistrationService.RegisterSpaApp(settings).ExecuteOperationAsync();
    }
}
