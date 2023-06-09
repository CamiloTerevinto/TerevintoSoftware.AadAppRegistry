﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.Models.ODataErrors;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AadAppRegistry.Tool.Commands;
using TerevintoSoftware.AadAppRegistry.Tool.Models;
using TerevintoSoftware.AadAppRegistry.Tool.Services;

namespace TerevintoSoftware.AadAppRegistry.Tool.Utilities;

[ExcludeFromCodeCoverage]
internal static class Startup
{
    internal static int RunSpectreConsole(string[] args)
    {
        var registrar = BuildTypeRegistrar();
        var app = new CommandApp(registrar);

        app.Configure(configurator =>
        {
            configurator
                .SetApplicationName("AadAppRegistry")
                .SetApplicationVersion("1.0.0");

            configurator.SetExceptionHandler(ex =>
            {
                if (ex is InvalidCredentialsException)
                {
                    AnsiConsole.MarkupLine("[bold red]Error:[/] credentials must be set first using the `configure credentials` command");
                }
                else if (ex is ODataError oDataError)
                {
                    AnsiConsole.MarkupLine($"[bold red]Error:[/] {oDataError.Error?.Message ?? oDataError.Message}");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[bold red]Error:[/] {ex.Message}");
                }

                return 1;
            });

            configurator.AddBranch("configure", configureBranch =>
            {
                configureBranch.SetDescription("Contains commands to change the configuration used for subsequent commands.");

                configureBranch.AddCommand<ConfigureGeneralSettingsCommand>("mode")
                    .WithDescription("Configures the mode (AAD or B2C) to use for app registrations.")
                    .WithExample(new[] { "configure", "mode", "--use-b2c", "false" });

                configureBranch.AddCommand<ConfigureCredentialsCommand>("credentials")
                    .WithDescription("Configures the client credentials to use to communicate with Microsoft Graph API.")
                    .WithExample(new[] { "configure", "credentials", "-t", "65cc525e-9959-461c-b42e-f2fb90fcd73f", "-c", "2e377341-710f-4a22-9604-bf4e02f5b1e2", "-s", "AzureGeneratedSecret" })
                    .WithExample(new[] { "configure", "credentials", "-t", "your_tenant_name.onmicrosoft.com", "-c", "2e377341-710f-4a22-9604-bf4e02f5b1e2", "-s", "AzureGeneratedSecret" });
            });

            configurator.AddCommand<ValidateCommand>("validate")
                .WithDescription("Validates that the credentials stored using `configure credentials` can be used to connect to Microsoft Graph.")
                .WithExample(new[] { "validate" });

            configurator.AddBranch("publish", publishBranch =>
            {
                publishBranch.SetDescription("Contains commands to publish new applications.");

                publishBranch.AddCommand<PublishApiAppCommand>("api")
                    .WithDescription("[[API]] Publishes an API app registration")
                    .WithExample(new[] { "publish", "api", "Some.Name.Api", "--set-default-uri", "--access-as-user" })
                    .WithExample(new[] { "publish", "api", "Some.Name.Api", "--set-app-uri", "https://contoso.onmicrosoft.com/Some.Name.Api", "--access-as-user" });

                publishBranch.AddCommand<PublishWebAppCommand>("web")
                    .WithDescription("[[Implicit/Hybrid]] Publishes a classic Web (server-side frameworks) app registration")
                    .WithExample(new[] { "publish", "web", "Some.Name.Web", "--redirect-uris", "http://localhost:1234/oauth2/redirect" });

                publishBranch.AddCommand<PublishSpaAppCommand>("spa")
                    .WithDescription("[[PKCE]] Publishes an SPA (client-side frameworks) app registration")
                    .WithExample(new[] { "publish", "spa", "Some.Name.Spa", "--redirect-uris", "http://localhost:1234/oauth2/redirect" });

                publishBranch.AddCommand<PublishConfidentialAppCommand>("confidential")
                    .WithDescription("[[Client Credentials]] Publishes a confidential app registration")
                    .WithExample(new[] { "publish", "confidential", "Some.Name.Confidential", "-s", "-e", "180" })
                    .WithExample(new[] { "publish", "confidential", "Some.Name.Confidential", "-s", "-k", "https://{YOUR_VAULT_NAME}.vault.azure.net/", "--dots-as-dashes" })
                    .WithExample(new[] { "publish", "confidential", "Some.Name.Confidential", "-s", "-k", "https://{YOUR_VAULT_NAME}.vault.azure.net/", "-n", "some-name-confidential-secret" });

                publishBranch.AddCommand<PublishNativeAppCommand>("native")
                    .WithDescription("[[Device Code]] Publishes a native (desktop/mobile) app registration")
                    .WithExample(new[] { "publish", "native", "Some.Name.Spa", "--redirect-uris", "myapp://oauth2/redirect" });
            });

            configurator.AddBranch("app", appBranch =>
            {
                appBranch.SetDescription("Contains commands to work with existing applications.");

                appBranch.AddCommand<ViewAppDetailsCommand>("view")
                    .WithDescription("Shows the details of an application")
                    .WithExample(new[] { "app", "view", "65cc525e-9959-461c-b42e-f2fb90fcd73f" })
                    .WithExample(new[] { "app", "view", "Some.Web.App" });

                appBranch.AddCommand<AppAddScopeCommand>("add-scope")
                    .WithDescription("Adds a scope to the application provided")
                    .WithExample(new[] { "app", "add-scope", "65cc525e-9959-461c-b42e-f2fb90fcd73f", "--api-app-id", "65cc525e-9959-461c-abcd-f2fb90fcd73f", "--scope-name", "test" })
                    .WithExample(new[] { "app", "add-scope", "Some.Web.App", "--api-app-id", "Some.Api.App", "--scope-name", "test" });

                appBranch.AddCommand<AppDeleteCommand>("delete")
                    .WithDescription("Deletes an application")
                    .WithExample(new[] { "app", "delete", "65cc525e-9959-461c-b42e-f2fb90fcd73f", "-y" })
                    .WithExample(new[] { "app", "delete", "Some.Web.App", "-y" });
            });

            configurator.AddCommand<ListApplicationsCommand>("list")
                .WithDescription("Lists all applications in the tenant, using the provided configuration")
                .WithExample(new[] { "list", "--take", "50", "--order-by", "displayName" });
        });

        return app.Run(args);
    }

    internal static ITypeRegistrar BuildTypeRegistrar()
    {
        var services = new ServiceCollection()
            .AddSingleton<IConfigurationService, ConfigurationService>()
            .AddSingleton<IGraphServiceClientFactory, GraphServiceClientFactory>()
            .AddSingleton<IGraphClientService, GraphClientService>()
            .AddSingleton<IAppRegistrationService, AppRegistrationService>()
            .AddSingleton<IAppConfigurationService, AppConfigurationService>()
            .AddSingleton<IKeyVaultClientService, KeyVaultClientService>();

        return new TypeRegistrar(services);
    }
}
