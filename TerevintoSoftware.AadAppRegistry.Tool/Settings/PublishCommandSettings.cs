using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

public enum OutputType
{
    Json
}

public enum SignInAudienceType
{
    AzureADMyOrg,
    AzureADMultipleOrgs,
    AzureADandPersonalMicrosoftAccount,
    PersonalMicrosoftAccount
}

public abstract class PublishCommandSettings : CommandSettings
{
    [CommandArgument(0, "[APP_NAME]")]
    [Description("The name of the application")]
    public string ApplicationName { get; set; }

    [CommandOption("-o|--output")]
    [Description("How the output of the Publish commands is formatted on success. Options: json")]
    public OutputType OutputType { get; set; }

    [CommandOption("--disable-duplicate-check")]
    [Description("If enabled, disables a check for an existing application with the same name")]
    public bool DisableDuplicateCheck { get; set; }

    [CommandOption("--sign-in-audience")]
    [Description("The valid audience for this client. Options: AzureADMyOrg [default], AzureADMultipleOrgs, AzureADandPersonalMicrosoftAccount, and PersonalMicrosoftAccount")]
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

public abstract class PublishClientCommandSettings : PublishCommandSettings
{
    [CommandOption("--consumed-scopes")]
    [Description("The scopes that are consumed by this client")]
    public string[] ConsumedScopes { get; set; }
}

public class PublishApiCommandSettings : PublishCommandSettings
{
    [CommandOption("--set-default-uri")]
    [Description("Sets the default URI for the application")]
    public bool SetDefaultUri { get; set; }

    [CommandOption("--set-app-uri")]
    [Description("Sets the specified URI. Mutually exclusive with --set-default-uri")]
    public string SetApplicationUri { get; set; }

    [CommandOption("--access-as-user")]
    [Description("Adds the access_as_user scope")]
    public bool SetDefaultScope { get; set; }

    [CommandOption("--scopes")]
    [Description("The scopes that the API publishes")]
    public string[] Scopes { get; set; }
}

public class PublishWebCommandSettings : PublishClientCommandSettings
{
    [CommandOption("--redirect-uris")]
    [Description("The redirect URIs that are valid for this client")]
    public string[] RedirectUris { get; set; }
}

public class PublishSpaCommandSettings : PublishClientCommandSettings
{
    [CommandOption("--redirect-uris")]
    [Description("The redirect URIs that are valid for this client")]
    public string[] RedirectUris { get; set; }
}

public class PublishConfidentialCommandSettings : PublishClientCommandSettings
{
    [CommandOption("--with-client-secret")]
    [Description("Creates a client secret")]
    public bool CreateClientSecret { get; set; }

    [CommandOption("--secret-expiration-days")]
    [Description("The number in days after which the client secret expires. Defaults to 180 days.")]
    public int ClientSecretExpirationDays { get; set; }
}
