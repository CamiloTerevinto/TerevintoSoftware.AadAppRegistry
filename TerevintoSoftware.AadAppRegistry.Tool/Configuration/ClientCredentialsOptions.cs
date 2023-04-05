﻿namespace TerevintoSoftware.AadAppRegistry.Tool.Configuration;

public class ClientCredentialsOptions
{
    public string TenantId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }    

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(TenantId) && !string.IsNullOrEmpty(ClientId) && !string.IsNullOrEmpty(ClientSecret);
    }
}
