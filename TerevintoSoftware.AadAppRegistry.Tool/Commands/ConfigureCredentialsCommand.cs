using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Services;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

public class ConfigureCredentialsCommand : Command<ClientCredentialsSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] ClientCredentialsSettings settings)
    {
        if (settings.DisableConfigurationSave == true)
        {
            AnsiConsole.MarkupLine($"[yellow]Warning:[/] calling [underline]configure credentials[/] with configuration saving disabled has no effect.");
            return -1;
        }

        AppRegistryConfiguration.Instance.ClientCredentials = new ClientCredentialsOptions
        {
            ClientId = settings.ClientId,
            ClientSecret = settings.ClientSecret,
            TenantId = settings.TenantId,
        };

        var service = new ConfigurationService();

        service.Save();

        return 0;
    }

    public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] ClientCredentialsSettings settings)
    {
        var error = "";

        if (string.IsNullOrEmpty(settings.TenantId))
        {
            error += "A value for --tenant-id must be specified. ";
        }

        if (string.IsNullOrEmpty(settings.ClientId))
        {
            error += "A value for --client-id must be specified. ";
        }

        if (string.IsNullOrEmpty(settings.ClientSecret))
        {
            error += "A value for --client-secret must be specified.";
        }

        if (error != "")
        {
            return ValidationResult.Error(error);
        }

        return base.Validate(context, settings);
    }
}

public class ClientCredentialsSettings : CommandSettings
{
    [CommandOption("-t|--tenant-id")]
    [Description("The ID of the Tenant where the app is registered")]
    public string TenantId { get; init; }

    [CommandOption("-c|--client-id")]
    [Description("The ID of the client")]
    public string ClientId { get; init; }

    [CommandOption("-s|--client-secret")]
    [Description("The secret to authenticate the client")]
    public string ClientSecret { get; init; }

    [CommandOption("--disable-configuration-save")]
    [Description("Whether to disable saving this configuration to disk")]
    public bool? DisableConfigurationSave { get; set; }
}