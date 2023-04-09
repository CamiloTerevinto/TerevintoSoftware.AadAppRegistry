using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

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

    public override ValidationResult Validate()
    {
        var error = "";

        if (string.IsNullOrEmpty(TenantId))
        {
            error += "A value for --tenant-id must be specified. ";
        }

        if (string.IsNullOrEmpty(ClientId))
        {
            error += "A value for --client-id must be specified. ";
        }

        if (string.IsNullOrEmpty(ClientSecret))
        {
            error += "A value for --client-secret must be specified.";
        }

        if (error != "")
        {
            return ValidationResult.Error(error);
        }

        return ValidationResult.Success();
    }
}