using Azure.Identity;
using Spectre.Console.Cli;
using TerevintoSoftware.AadAppRegistry.Tool.Configuration;
using TerevintoSoftware.AadAppRegistry.Tool.Services;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tool.Commands;

public class ValidateCommand : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var clientCredentials = new ClientCertificateCredential("4b25c54b-1011-467c-bdbb-ddd0f1dfd1df",
            "3568d7f8-fae5-423d-b66a-723d18ab4c97", "aEx8Q~HcaxEUce.KE9g~dma270azGdtY2OUsQc4b");

        //var service = new AppRegistrationsGraphService(GraphServiceClientFactory.CreateClient(AppRegistryConfiguration.Instance));
        var config = new ConfigurationService();
        config.Load();
        var service = new AppRegistrationsGraphService(GraphServiceClientFactory.CreateClient(AppRegistryConfiguration.Instance));

        await service.ValidateConnection();

        return 0;
    }
}
