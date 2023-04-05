using Spectre.Console.Cli;
using System.ComponentModel;

namespace TerevintoSoftware.AadAppRegistry.Tool.Settings;

public class ConfigureModeSettings : CommandSettings
{
    [CommandOption("--use-b2c")]
    [Description("Whether to use the AAD (false) or B2C (true) mode")]
    public bool UseB2CMode { get; init; }
}