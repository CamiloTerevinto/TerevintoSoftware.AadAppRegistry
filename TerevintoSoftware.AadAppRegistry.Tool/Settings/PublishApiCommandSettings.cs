using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using TerevintoSoftware.AadAppRegistry.Tool.Settings.Base;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

public class PublishApiCommandSettings : PublishCommandBaseSettings
{
    [CommandOption("--set-default-uri")]
    [Description("Sets the default URI for the application. AAD mode will use [bold royalblue1]api://{id}[/] while B2C mode will use [bold royalblue1]https://{tenant}.onmicrosoft.com/{id}[/].")]
    public bool SetDefaultApplicationUri { get; set; }

    [CommandOption("--app-uri")]
    [Description("Sets the specified URI. [darkorange]Mutually exclusive with --set-default-uri[/]")]
    public string ApplicationUri { get; set; }

    [CommandOption("--access-as-user")]
    [Description("Adds the access_as_user scope. Requires the application uri to be set.")]
    public bool SetDefaultScope { get; set; }

    public override ValidationResult Validate()
    {
        if (SetDefaultApplicationUri && !string.IsNullOrEmpty(ApplicationUri))
        {
            return ValidationResult.Error("You can only specify one of --set-default-uri or --app-uri.");            
        }

        if (!SetDefaultApplicationUri && string.IsNullOrEmpty(ApplicationUri) && SetDefaultScope)
        {
            return ValidationResult.Error("Setting the access_as_user scope requires the app uri to be set with --set-default-uri or --app-uri.");
        }

        return base.Validate();
    }
}
