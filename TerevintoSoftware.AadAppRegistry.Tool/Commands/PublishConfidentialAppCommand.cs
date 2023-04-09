using Spectre.Console;
using Spectre.Console.Cli;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

internal class PublishConfidentialAppCommand : AsyncCommand<PublishConfidentialCommandSettings>
{
    private readonly IAppRegistrationService _appRegistrationService;

    public PublishConfidentialAppCommand(IAppRegistrationService appRegistrationService)
    {
        _appRegistrationService = appRegistrationService;
    }

    public override Task<int> ExecuteAsync(CommandContext context, PublishConfidentialCommandSettings settings)
    {
        AnsiConsole.Markup("[bold red]Error:[/] this command has not yet been implemented.");
        return Task.FromResult(1);
        //AnsiConsole.WriteLine(JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
        //return Task.FromResult(0);
    }
}