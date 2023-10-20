using Microsoft.Graph;
using Microsoft.Graph.Applications.GetByIds;
using Microsoft.Graph.Applications.Item.AddPassword;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using TerevintoSoftware.AadAppRegistry.Tool.Settings;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IGraphClientService
{
    Task<ServiceOperationResult> AddConsumedScopeByIdAsync(Application application, string originalAppId, Guid scopeId);
    Task<ServiceOperationResult> AddApiScopeAsync(Application application, string scopeName, string scopeDisplayName, string scopeDescription);
    Task<ServiceOperationResult<string>> AddClientSecretAsync(Application application, DateTimeOffset? expirationTime);
    Task<ServiceOperationResult> AddNativeRedirectUrisAsync(Application application, string[] redirectUris);
    Task<ServiceOperationResult> AddSpaRedirectUrisAsync(Application application, string[] redirectUris);
    Task<ServiceOperationResult> AddWebRedirectUrisAsync(Application application, string[] redirectUris);
    Task<ServiceOperationResult<Application>> CreateApplicationAsync(string displayName, SignInAudienceType signInAudienceType);
    Task<ServiceOperationResult<Application>> GetApplicationByDisplayNameAsync(string displayName);
    Task<ServiceOperationResult<Application>> GetApplicationByIdAsync(string clientId);
    Task<ServiceOperationResult<Application>> GetApplicationByIdOrNameAsync(string clientIdOrDisplayName);
    Task<ServiceOperationResult> SetApplicationIdUriAsync(Application application, string uri);
    Task<ServiceOperationResult> ValidateConnectionAsync();
    Task<ServiceOperationResult> DeleteApplicationAsync(Application application);
    Task<ServiceOperationResult<ApplicationCollectionResponse>> GetApplicationsAsync(int take, string orderBy);
    Task<ServiceOperationResult<List<(string, string)>>> GetScopeNamesAsync(List<(string, Guid)> scopes);
}

internal class GraphClientService : IGraphClientService
{
    private readonly IGraphServiceClientFactory _factory;

    private GraphServiceClient GraphClient => _factory.CreateClient();

    public GraphClientService(IGraphServiceClientFactory graphServiceClientFactory)
    {
        _factory = graphServiceClientFactory;
    }

    public async Task<ServiceOperationResult> ValidateConnectionAsync()
    {
        return await RunGraphQueryAsync(async () =>
        {
            var randomClient = await GraphClient.Applications.GetAsync(r =>
            {
                r.QueryParameters.Top = 1;
                r.QueryParameters.Select = new[] { "appId" };
            });
        });
    }

    public async Task<ServiceOperationResult<Application>> GetApplicationByIdOrNameAsync(string clientIdOrDisplayName)
    {
        var getApplicationResult = await GetApplicationByIdAsync(clientIdOrDisplayName);

        if (getApplicationResult.Data != null || !getApplicationResult.Success)
        {
            return getApplicationResult;
        }

        return await GetApplicationByDisplayNameAsync(clientIdOrDisplayName);
    }

    public async Task<ServiceOperationResult<Application>> GetApplicationByIdAsync(string clientId)
    {
        return await RunGraphQueryAsync(async () =>
        {
            var apps = await GraphClient.Applications.GetAsync(r =>
            {
                r.QueryParameters.Filter = $"appId eq '{clientId}'";
                r.QueryParameters.Top = 1;
                r.Headers.Add("ConsistencyLevel", "eventual");
                r.QueryParameters.Count = true;
            });
            return apps.Value.Count == 1 ? apps.Value[0] : null;
        });
    }

    public async Task<ServiceOperationResult<Application>> GetApplicationByDisplayNameAsync(string displayName)
    {
        return await RunGraphQueryAsync(async () =>
        {
            var apps = await GraphClient.Applications.GetAsync(r =>
            {
                r.QueryParameters.Filter = $"displayName eq '{displayName}'";
                r.QueryParameters.Top = 1;
            });

            return apps.Value.Count == 1 ? apps.Value[0] : null;
        });
    }

    public async Task<ServiceOperationResult<ApplicationCollectionResponse>> GetApplicationsAsync(int take, string orderBy)
    {
        return await RunGraphQueryAsync(async () =>
        {
            var apps = await GraphClient.Applications.GetAsync(r =>
            {
                r.QueryParameters.Top = take;
                r.Headers.Add("ConsistencyLevel", "eventual");
                r.QueryParameters.Count = true;
                r.QueryParameters.Orderby = new[] { orderBy };
            });

            return apps;
        });
    }

    public async Task<ServiceOperationResult<Application>> CreateApplicationAsync(string displayName, SignInAudienceType signInAudienceType)
    {
        var appToBeCreated = new Application
        {
            DisplayName = displayName,
            SignInAudience = signInAudienceType.ToString(),
        };

        return await RunGraphQueryAsync(() => GraphClient.Applications.PostAsync(appToBeCreated));
    }

    public async Task<ServiceOperationResult> SetApplicationIdUriAsync(Application application, string uri)
    {
        var toUpdate = new Application
        {
            IdentifierUris = new() { uri }
        };

        return await RunGraphQueryAsync(() => GraphClient.Applications[application.Id].PatchAsync(toUpdate));
    }

    public async Task<ServiceOperationResult> AddApiScopeAsync(Application application, string scopeName, string scopeDisplayName, string scopeDescription)
    {
        if (application.Api.Oauth2PermissionScopes.Any(x => x.Value == scopeName))
        {
            return ServiceOperationResultStatus.Success;
        }

        var updatedApp = new Application
        {
            Api = new ApiApplication
            {
                Oauth2PermissionScopes = new()
                {
                    new PermissionScope
                    {
                        AdminConsentDisplayName = scopeDisplayName,
                        AdminConsentDescription = scopeDescription,
                        Value = scopeName,
                        IsEnabled = true,
                        Id = Guid.NewGuid(),
                        Type = "Admin"
                    }
                },
                RequestedAccessTokenVersion = 2
            }
        };

        return await RunGraphQueryAsync(() => GraphClient.Applications[application.Id].PatchAsync(updatedApp));
    }

    public async Task<ServiceOperationResult> AddSpaRedirectUrisAsync(Application application, string[] redirectUris)
    {
        var redirectUrisToSet = application.Spa.RedirectUris.Union(redirectUris).ToHashSet();

        if (!redirectUris.Any())
        {
            return ServiceOperationResultStatus.Success;
        }

        var updatedApp = new Application
        {
            Spa = new SpaApplication
            {
                RedirectUris = redirectUrisToSet.ToList()
            }
        };

        return await RunGraphQueryAsync(() => GraphClient.Applications[application.Id].PatchAsync(updatedApp));
    }

    public async Task<ServiceOperationResult> AddWebRedirectUrisAsync(Application application, string[] redirectUris)
    {
        var redirectUrisToSet = application.Web.RedirectUris.Union(redirectUris).ToHashSet();

        if (!redirectUris.Any())
        {
            return ServiceOperationResultStatus.Success;
        }

        var updatedApp = new Application
        {
            Web = new WebApplication
            {
                RedirectUris = redirectUrisToSet.ToList(),
            }
        };

        return await RunGraphQueryAsync(() => GraphClient.Applications[application.Id].PatchAsync(updatedApp));
    }

    public async Task<ServiceOperationResult> AddNativeRedirectUrisAsync(Application application, string[] redirectUris)
    {
        var redirectUrisToSet = application.PublicClient.RedirectUris.Union(redirectUris).ToHashSet();

        if (!redirectUris.Any())
        {
            return ServiceOperationResultStatus.Success;
        }

        var updatedApp = new Application
        {
            PublicClient = new PublicClientApplication
            {
                RedirectUris = redirectUrisToSet.ToList()
            }
        };

        return await RunGraphQueryAsync(() => GraphClient.Applications[application.Id].PatchAsync(updatedApp));
    }

    public async Task<ServiceOperationResult<string>> AddClientSecretAsync(Application application, DateTimeOffset? expirationTime)
    {
        var requestBody = new AddPasswordPostRequestBody
        {
            PasswordCredential = new PasswordCredential
            {
                DisplayName = "Client secret",
                EndDateTime = expirationTime ?? DateTimeOffset.UtcNow.AddMonths(6)
            }
        };

        return await RunGraphQueryAsync(async () =>
        {
            var result = await GraphClient.Applications[application.Id].AddPassword.PostAsync(requestBody);

            return result.SecretText;
        });
    }

    public async Task<ServiceOperationResult> AddConsumedScopeByIdAsync(Application application, string originalAppId, Guid scopeId)
    {
        if (application.RequiredResourceAccess.Any(x => x.ResourceAppId == originalAppId && x.ResourceAccess.Any(y => y.Id == scopeId)))
        {
            return ServiceOperationResultStatus.Success;
        }

        var updatedApp = new Application
        {
            RequiredResourceAccess = new()
            {
                new()
                {
                    ResourceAppId = originalAppId,
                    ResourceAccess = new()
                    {
                        new()
                        {
                            Id = scopeId,
                            Type = "Scope",
                        }
                    }
                }
            }
        };

        return await RunGraphQueryAsync(() => GraphClient.Applications[application.Id].PatchAsync(updatedApp));
    }

    public async Task<ServiceOperationResult> DeleteApplicationAsync(Application application)
    {
        return await RunGraphQueryAsync(() => GraphClient.Applications[application.Id].DeleteAsync());
    }

    public async Task<ServiceOperationResult<List<(string, string)>>> GetScopeNamesAsync(List<(string, Guid)> scopes)
    {
        var appIds = scopes.Select(x => x.Item1).Distinct().ToList();

        var result = await RunGraphQueryAsync(async () =>
        {
            var apps = await GraphClient.Applications.GetAsync(r =>
            {
                r.QueryParameters.Filter = $"appId eq '{string.Join("' or appId eq '", appIds)}'";
                r.QueryParameters.Select = new[] { "appId", "api", "api.oauth2PermissionScopes" };
            });

            return apps.Value;
        });

        if (!result.Success)
        {
            return new(result);
        }

        return result.Data
            .SelectMany(x => x.Api.Oauth2PermissionScopes.Select(y => (x.AppId, ScopeId: y.Id, ScopeName: y.Value)).ToArray())
            .Where(x => scopes.Any(y => y.Item2 == x.ScopeId))
            .Select(x => (x.AppId, x.ScopeName))
            .ToList();
    }

    private static async Task<ServiceOperationResult> RunGraphQueryAsync(Func<Task> action)
    {
        try
        {
            await action();

            return ServiceOperationResultStatus.Success;
        }
        catch (ODataError e)
        {
            return new(ServiceOperationResultStatus.Failed, e.Error.Message);
        }
        catch (Exception ex)
        {
            return new(ServiceOperationResultStatus.Failed, ex.Message);
        }
    }

    private static async Task<ServiceOperationResult<T>> RunGraphQueryAsync<T>(Func<Task<T>> action) where T : class
    {
        try
        {
            return await action();
        }
        catch (ODataError e)
        {
            return new(ServiceOperationResultStatus.Failed, e.Error.Message);
        }
        catch (Exception ex)
        {
            return new(ServiceOperationResultStatus.Failed, ex.Message);
        }
    }
}
