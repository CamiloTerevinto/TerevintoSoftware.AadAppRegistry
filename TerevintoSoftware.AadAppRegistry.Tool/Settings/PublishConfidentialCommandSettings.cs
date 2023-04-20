using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

public partial class PublishConfidentialCommandSettings : PublishCommandSettings
{
    [CommandOption("-s|--with-client-secret")]
    [Description("Creates a client secret")]
    public bool CreateClientSecret { get; set; }

    [CommandOption("-e|--secret-expiration-days")]
    [Description("The number in days after which the client secret expires. Defaults to 180 days.")]
    public int ClientSecretExpirationDays { get; set; }

    [CommandOption("-k|--key-vault")]
    [Description("[[Optional:]] the URI of the key vault to store the client secret in. The Service Principal is used for authenticating to the Key Vault.")]
    public string KeyVaultUri { get; set; }

    [CommandOption("-n|--secret-name")]
    [Description("[[Optional:]] the name of the secret in the key vault. Defaults to the name of the application.")]
    public string SecretName { get; set; }

    [CommandOption("--dots-as-dashes")]
    [Description("[[Optional:]] replaces dots in the application name with dashes. This is useful when using the application name as the secret name in the key vault.")]
    public bool DotsAsDashes { get; set; }

    public override ValidationResult Validate()
    {
        if (!string.IsNullOrEmpty(KeyVaultUri) && !Uri.TryCreate(KeyVaultUri, UriKind.Absolute, out _))
        {
            return ValidationResult.Error("The key vault URI is not valid.");
        }

        if (ClientSecretExpirationDays < 2)
        {
            return ValidationResult.Error("The client secret expiration days must be greater than 1.");
        }

        if (string.IsNullOrEmpty(SecretName))
        {
            SecretName = ApplicationName.Replace(".", "-");
        }

        // check if the secret name is valid according to the Key Vault naming rules
        if (!string.IsNullOrEmpty(KeyVaultUri) && !KeyVaultSecretNameRegEx().IsMatch(SecretName))
        {
            return ValidationResult.Error("The secret name is not valid. It must only contain alphanumeric characters and dashes.");
        }

        return base.Validate();
    }

    [GeneratedRegex("^[0-9a-zA-Z-]+$")]
    private static partial Regex KeyVaultSecretNameRegEx();
}
