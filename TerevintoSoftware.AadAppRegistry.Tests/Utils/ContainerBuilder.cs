using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tests.Utils;

internal static class ContainerBuilder
{
    internal static TService BuildServicesFor<TService>(OperatingMode operatingMode = OperatingMode.AzureActiveDirectory)
    {
        var registrar = (TypeRegistrar)Startup.BuildTypeRegistrar();
        registrar.Replace(typeof(IConfigurationService), new ConfigurationServiceMock(operatingMode));
        var resolver = registrar.Build();

        return (TService)resolver.Resolve(typeof(TService));
    }
}
