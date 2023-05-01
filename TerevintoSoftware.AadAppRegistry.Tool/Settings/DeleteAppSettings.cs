using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Settings.Base;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

[ExcludeFromCodeCoverage]
internal class DeleteAppSettings : ConfigureAppBaseSettings
{
    [CommandOption("-y")]
    [Description("Skip confirmation before deletion.")]
    public bool SuppressConfirmation { get; init; }
}
