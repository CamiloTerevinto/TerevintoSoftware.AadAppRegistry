using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

[ExcludeFromCodeCoverage]
public class ListAppsSettings : CommandSettings
{
    [CommandOption("--take")]
    [Description("The number of apps to take. Defaults to 50.")]
    public int Take { get; set; } = 50;

    [CommandOption("--order-by")]
    [Description("The property to use to order apps by. Defaults to displayName.")]
    public string OrderBy { get; set; } = "displayName";

}
