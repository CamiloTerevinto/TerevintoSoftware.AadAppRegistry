using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

[ExcludeFromCodeCoverage]
internal class PublishConfidentialAppCommand : AsyncCommand<PublishConfidentialCommandSettings>
{
    private readonly IAppRegistrationService _appRegistrationService;

    public PublishConfidentialAppCommand(IAppRegistrationService appRegistrationService)
    {
        _appRegistrationService = appRegistrationService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, PublishConfidentialCommandSettings settings)
    {
        return await _appRegistrationService.RegisterConfidentialAppAsync(settings).ExecuteOperationAsync();
    }
}