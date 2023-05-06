using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings.Base;

[ExcludeFromCodeCoverage]
public abstract class AppBranchBaseSettings : CommandSettings
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
