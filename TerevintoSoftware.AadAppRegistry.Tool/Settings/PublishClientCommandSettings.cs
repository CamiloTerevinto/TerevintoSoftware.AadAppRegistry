using Spectre.Console.Cli;
using System.ComponentModel;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

public abstract class PublishClientCommandSettings : PublishCommandSettings
{
    [CommandOption("--consumed-scopes")]
    [Description("The scopes that are consumed by this client")]
    public string[] ConsumedScopes { get; set; }
}
