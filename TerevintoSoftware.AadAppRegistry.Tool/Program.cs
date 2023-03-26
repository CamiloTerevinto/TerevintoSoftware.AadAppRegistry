using Spectre.Console.Cli;
using TerevintoSoftware.AadAppRegistry.Tool.Commands;

var app = new CommandApp();

app.Configure(appConfiguration =>
{
    appConfiguration
        .SetApplicationName("AadAppRegistry")
        .SetApplicationVersion("0.0.1");

    appConfiguration.AddBranch("configure", configure =>
    {
        configure.SetDescription("Allows you to change the configuration used for subsequent commands.");

        configure.AddCommand<ConfigureModeCommand>("mode")
            .WithDescription("Configures the mode (AAD or B2C) to use for app registration.")
            .WithExample(new[] { "configure", "mode", "--use-b2c" });

        configure.AddCommand<ConfigureCredentialsCommand>("credentials")
            .WithDescription("Configures the client credentials to use to communicate with Microsoft Graph API.")
            .WithExample(new[] { "configure", "credentials", "-t", "65cc525e-9959-461c-b42e-f2fb90fcd73f", "-c", "2e377341-710f-4a22-9604-bf4e02f5b1e2", "-s", "AzureGeneratedSecret" })
            .WithExample(new[] { "configure", "credentials", "-t", "your_tenant_name.onmicrosoft.com", "-c", "2e377341-710f-4a22-9604-bf4e02f5b1e2", "-s", "AzureGeneratedSecret" });
    });

    appConfiguration.AddCommand<ValidateCommand>("validate")
        .WithDescription("Validates that the credentials stored using `configure credentials` can be used to connect to Microsoft Graph.")
        .WithExample(new[] { "validate" });
});

return app.Run(args);
//return app.Run(new[] { "validate" });