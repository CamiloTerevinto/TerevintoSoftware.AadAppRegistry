using Spectre.Console.Cli;
using System.ComponentModel;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

public class GeneralConfigurationCommandSettings : CommandSettings
{
    [CommandOption("--use-b2c")]
    [Description("Whether to use the AAD (false) or B2C (true) mode")]
    public bool UseB2CMode { get; init; }

    [CommandOption("-t|--tenant-name")]
    [Description("The name of the tenant to use for B2C mode (without .onmicrosoft.com)")]
    public string TenantName { get; init; }
}