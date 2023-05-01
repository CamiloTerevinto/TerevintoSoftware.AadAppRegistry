using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Settings.Base;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

[ExcludeFromCodeCoverage]
public class PublishNativeCommandSettings : PublishCommandBaseSettings
{
    [CommandOption("--redirect-uris")]
    [Description("The redirect URIs that are valid for this client")]
    public string[] RedirectUris { get; set; }
}
