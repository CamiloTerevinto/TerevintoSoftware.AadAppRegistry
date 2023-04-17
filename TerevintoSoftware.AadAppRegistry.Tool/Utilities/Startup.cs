using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TerevintoSoftware.AadAppRegistry.Tool.Services;

namespace TerevintoSoftware.AadAppRegistry.Tool.Utilities;

internal static class Startup
{
    internal static ITypeRegistrar BuildTypeRegistrar()
    {
        var services = new ServiceCollection()
            .AddSingleton<IConfigurationService, ConfigurationService>()
            .AddSingleton<IGraphServiceClientFactory, GraphServiceClientFactory>()
            .AddSingleton<IGraphClientService, GraphClientService>()
            .AddSingleton<IAppRegistrationService, AppRegistrationService>()
            .AddSingleton<IAppConfigurationService, AppConfigurationService>();

        return new TypeRegistrar(services);
    }
}
