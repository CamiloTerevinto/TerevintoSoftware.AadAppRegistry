using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

[ExcludeFromCodeCoverage]
internal class PublishWebAppCommand : AsyncCommand<PublishWebCommandSettings>
{
    private readonly IAppRegistrationService _appRegistrationService;

    public PublishWebAppCommand(IAppRegistrationService appRegistrationService)
    {
        _appRegistrationService = appRegistrationService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, PublishWebCommandSettings settings)
    {
        return await _appRegistrationService.RegisterWebApp(settings).ExecuteOperationAsync();
    }
}
