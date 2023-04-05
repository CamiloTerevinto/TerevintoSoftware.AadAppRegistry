namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IAppRegistrationService
{

}

internal class AppRegistrationService : IAppRegistrationService
{
    private readonly IAppRegistrationsGraphService _graphService;

    public AppRegistrationService(IAppRegistrationsGraphService graphService)
    {
        _graphService = graphService;
    }
}
