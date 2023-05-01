using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using TerevintoSoftware.AadAppRegistry.Tool.Settings.Base;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

internal class ConfigureAppAddScopeSettings : ConfigureAppBaseSettings
{
    [CommandOption("--api-app-id")]
    [Description("The Client Id or Display Name of the API to be consumed")]
    public string ApiAppId { get; init; }

    [CommandOption("--scope-name")]
    [Description("The name of the scope to be added.")]
    public string ScopeName { get; init; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrEmpty(ApiAppId)) 
        { 
            return ValidationResult.Error("You must specify an API Client Id or Display Name with --api-app-id");
        }

        if (string.IsNullOrEmpty(ScopeName))
        {
            return ValidationResult.Error("You must specify a scope name with --scope-name");
        }

        return base.Validate();
    }
}
