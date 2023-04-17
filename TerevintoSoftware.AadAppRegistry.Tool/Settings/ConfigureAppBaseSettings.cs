using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

public abstract class ConfigureAppBaseSettings : CommandSettings
{
    [CommandArgument(0, "[APP]")]
    [Description("The Display Name or Client Id of the application")]
    public string ApplicationName { get; set; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrEmpty(ApplicationName))
        {
            return ValidationResult.Error("The Display Name or the Client Id of the application is required.");
        }

        return base.Validate();
    }
}
