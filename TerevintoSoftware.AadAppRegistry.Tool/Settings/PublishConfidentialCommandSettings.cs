using Spectre.Console.Cli;
using System.ComponentModel;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

public class PublishConfidentialCommandSettings : PublishClientCommandSettings
{
    [CommandOption("--with-client-secret")]
    [Description("Creates a client secret")]
    public bool CreateClientSecret { get; set; }

    [CommandOption("--secret-expiration-days")]
    [Description("The number in days after which the client secret expires. Defaults to 180 days.")]
    public int ClientSecretExpirationDays { get; set; }
}
