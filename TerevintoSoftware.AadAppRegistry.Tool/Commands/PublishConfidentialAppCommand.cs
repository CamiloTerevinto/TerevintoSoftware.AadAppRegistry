using Spectre.Console;
using Spectre.Console.Cli;
using System.Text.Json;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

internal class PublishConfidentialAppCommand : AsyncCommand<PublishWebCommandSettings>
{
    private readonly IAppRegistrationService _appRegistrationService;

    public PublishConfidentialAppCommand(IAppRegistrationService appRegistrationService)
    {
        _appRegistrationService = appRegistrationService;
    }

    public override Task<int> ExecuteAsync(CommandContext context, PublishWebCommandSettings settings)
    {
        AnsiConsole.WriteLine(JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
        return Task.FromResult(0);
    }
}