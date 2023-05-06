using Spectre.Console;
using Spectre.Console.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tool;

[ExcludeFromCodeCoverage]
internal static class ServiceOperationResultExtensions
{
    public static async Task<int> ExecuteOperationAsync(this Task<ServiceOperationResult> task)
    {
        var result = await task;

        AnsiConsole.Write(new JsonText(JsonSerializer.Serialize(result)));

        return result.Success ? 0 : 1;
    }

    public static async Task<int> ExecuteOperationAsync<T>(this Task<ServiceOperationResult<T>> task) where T : class
    {
        var result = await task;

        AnsiConsole.Write(new JsonText(JsonSerializer.Serialize(result)));

        return result.Success ? 0 : 1;
    }
}
