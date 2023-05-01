using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings.Base;

public abstract class PublishCommandBaseSettings : CommandSettings
{
    [CommandArgument(0, "[APP_NAME]")]
    [Description("The name of the application")]
    public string ApplicationName { get; set; }

    [CommandOption("--disable-duplicate-check")]
    [Description("If enabled, disables a check for an existing application with the same name")]
    public bool DisableDuplicateCheck { get; set; }

    [CommandOption("--sign-in-audience")]
    [Description("The valid audience for this client. Options: AzureADMyOrg [[default]], AzureADMultipleOrgs, AzureADandPersonalMicrosoftAccount, and PersonalMicrosoftAccount")]
    public SignInAudienceType SignInAudience { get; set; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrEmpty(ApplicationName))
        {
            return ValidationResult.Error("The application name is required");
        }

        return base.Validate();
    }
}
