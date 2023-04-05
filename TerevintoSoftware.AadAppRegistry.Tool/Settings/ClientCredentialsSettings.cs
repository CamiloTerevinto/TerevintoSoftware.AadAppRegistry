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

    //[CommandOption("--disable-configuration-save")]
    //[Description("Whether to disable saving this configuration to disk")]
    //public bool? DisableConfigurationSave { get; set; }
}