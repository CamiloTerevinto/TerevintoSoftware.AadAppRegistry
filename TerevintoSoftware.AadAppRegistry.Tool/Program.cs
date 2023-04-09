using Spectre.Console.Cli;
using TerevintoSoftware.AadAppRegistry.Tool.Commands;
using TerevintoSoftware.AadAppRegistry.Tool.Utilities;

var registrar = Startup.BuildTypeRegistrar();
var app = new CommandApp(registrar);

app.Configure(appConfiguration =>
{
    appConfiguration
        .SetApplicationName("AadAppRegistry")
        .SetApplicationVersion("0.0.1");

    appConfiguration.AddBranch("configure", configure =>
    {
        configure.SetDescription("Allows you to change the configuration used for subsequent commands.");

        configure.AddCommand<ConfigureModeCommand>("mode")
            .WithDescription("Configures the mode (AAD or B2C) to use for app registrations.")
            .WithExample(new[] { "configure", "mode", "--use-b2c", "false" });

        configure.AddCommand<ConfigureCredentialsCommand>("credentials")
            .WithDescription("Configures the client credentials to use to communicate with Microsoft Graph API.")
            .WithExample(new[] { "configure", "credentials", "-t", "65cc525e-9959-461c-b42e-f2fb90fcd73f", "-c", "2e377341-710f-4a22-9604-bf4e02f5b1e2", "-s", "AzureGeneratedSecret" })
            .WithExample(new[] { "configure", "credentials", "-t", "your_tenant_name.onmicrosoft.com", "-c", "2e377341-710f-4a22-9604-bf4e02f5b1e2", "-s", "AzureGeneratedSecret" });
    });

    appConfiguration.AddCommand<ValidateCommand>("validate")
        .WithDescription("Validates that the credentials stored using `configure credentials` can be used to connect to Microsoft Graph.")
        .WithExample(new[] { "validate" });

    appConfiguration.AddBranch("publish", publish =>
    {
        publish.SetDescription("Contains commands to publish to Azure App Registrations.");

        publish.AddCommand<PublishApiAppCommand>("api")
            .WithDescription("[[API]] Publishes an API app registration")
            .WithExample(new[] { "publish", "api", "Some.Name.Api", "--set-default-uri", "--access-as-user" })
            .WithExample(new[] { "publish", "api", "Some.Name.Api", "--set-app-uri", "https://contoso.onmicrosoft.com/Some.Name.Api", "--access-as-user" });

        publish.AddCommand<PublishWebAppCommand>("web")
            .WithDescription("[[Implicit/Hybrid mode]] Publishes a classic Web (server-side frameworks) app registration")
            .WithExample(new[] { "publish", "web", "Some.Name.Web", "--redirect-uris", "http://localhost:1234/oauth2/redirect", "--consumed-scopes", "api://{ClientId}/.default" })
            .WithExample(new[] { "publish", "web", "Some.Name.Web", "--redirect-uris", "http://localhost:1234/oauth2/redirect", "--consumed-scopes", "https://contoso.onmicrosoft.com/.default" });

        publish.AddCommand<PublishSpaAppCommand>("spa")
            .WithDescription("[[PKCE]] Publishes an SPA (client-side frameworks) app registration")
            .WithExample(new[] { "publish", "spa", "Some.Name.Spa", "--redirect-uris", "http://localhost:1234/oauth2/redirect", "--consumed-scopes", "api://{ClientId}/.default" })
            .WithExample(new[] { "publish", "spa", "Some.Name.Spa", "--redirect-uris", "http://localhost:1234/oauth2/redirect", "--consumed-scopes", "https://contoso.onmicrosoft.com/.default" });

        publish.AddCommand<PublishConfidentialAppCommand>("confidential")
            .WithDescription("[[Client Credentials]] Publishes a confidential app registration")
            .WithExample(new[] { "publish", "confidential", "Some.Name.Confidential", "--with-client-secret", "--consumed-scopes", "api://{ClientId}/.default" })
            .WithExample(new[] { "publish", "confidential", "Some.Name.Confidential", "--with-client-secret", "--consumed-scopes", "https://contoso.onmicrosoft.com/.default" });
    });
});

return app.Run(args);