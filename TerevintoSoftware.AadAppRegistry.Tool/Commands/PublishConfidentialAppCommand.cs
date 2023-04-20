using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;
using System.Text.Json;
using TerevintoSoftware.AadAppRegistry.Tool.Models;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

internal class PublishConfidentialAppCommand : AsyncCommand<PublishConfidentialCommandSettings>
{
    private readonly IAppRegistrationService _appRegistrationService;
    private readonly IKeyVaultClientService _keyVaultClientService;

    public PublishConfidentialAppCommand(IAppRegistrationService appRegistrationService, IKeyVaultClientService keyVaultClientService)
    {
        _appRegistrationService = appRegistrationService;
        _keyVaultClientService = keyVaultClientService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, PublishConfidentialCommandSettings settings)
    {
        var result = await _appRegistrationService.RegisterConfidentialAppAsync(settings);

        if (result.Success)
        {
            var keyVaultStatus = await PushSecretToKeyVaultAsync(settings, result.Data);

            if (keyVaultStatus != OperationResultStatus.Success)
            {
                return 1;
            }
        }

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

    private async Task<OperationResultStatus> PushSecretToKeyVaultAsync(PublishConfidentialCommandSettings settings, ConfidentialAppRegistrationResult result)
    {
        if (result.Secret == null || string.IsNullOrEmpty(settings.KeyVaultUri))
        {
            return OperationResultStatus.Success;
        }

        try
        {
            var expirationDate = DateTime.UtcNow.Date.AddDays(settings.ClientSecretExpirationDays);
            var secretUri = await _keyVaultClientService.PushClientSecretAsync(new Uri(settings.KeyVaultUri), settings.SecretName, result.Secret, expirationDate);
            result.Secret = secretUri.ToString();

            return OperationResultStatus.Success;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Error:[/] while the application was registered, pushing the secret to Key Vault failed: {ex.Message}.");
            AnsiConsole.Write(new JsonText(JsonSerializer.Serialize(result)));

            return OperationResultStatus.Failed;
        }
    }
}