using Spectre.Console.Cli;
using System.ComponentModel;
using TerevintoSoftware.AadAppRegistry.Tool.Settings.Base;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

public class PublishSpaCommandSettings : PublishCommandBaseSettings
{
    [CommandOption("--redirect-uris")]
    [Description("The redirect URIs that are valid for this client")]
    public string[] RedirectUris { get; set; }
}
