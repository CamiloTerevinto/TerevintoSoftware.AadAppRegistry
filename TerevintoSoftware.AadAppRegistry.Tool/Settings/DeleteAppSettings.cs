using Spectre.Console.Cli;
using System.ComponentModel;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

internal class DeleteAppSettings : ConfigureAppBaseSettings
{
    [CommandOption("-y")]
    [Description("Skip confirmation before deletion.")]
    public bool SuppressConfirmation { get; init; }
}
