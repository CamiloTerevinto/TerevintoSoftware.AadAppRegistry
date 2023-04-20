using Azure.Security.KeyVault.Secrets;

namespace TerevintoSoftware.AadAppRegistry.Tool.Services;

internal interface IKeyVaultClientService
{
    Task<Uri> PushClientSecretAsync(Uri keyVaultUri, string secretName, string secretValue, DateTimeOffset expirationDate);
}

internal class KeyVaultClientService : IKeyVaultClientService
{
    private readonly IConfigurationService _configurationService;

    public KeyVaultClientService(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public async Task<Uri> PushClientSecretAsync(Uri keyVaultUri, string secretName, string secretValue, DateTimeOffset expirationDate)
    {
        var client = new SecretClient(keyVaultUri, _configurationService.GetAzureCredential());
        var secret = new KeyVaultSecret(secretName, secretValue);
        secret.Properties.ExpiresOn = expirationDate;

        var response = await client.SetSecretAsync(secret);

        return response.Value.Id;
    }
}
