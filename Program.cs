using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using GolioFunctions.Services;
using SendGrid.Extensions.DependencyInjection;
using Azure.Identity;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        // Configurar fontes de configuração, por exemplo, JSON, variáveis de ambiente, etc.
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("local.settings.json", optional: false, reloadOnChange: true);
        config.AddEnvironmentVariables();

    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddLogging();
        services.AddScoped<IEmailService, EmailService>();
    })
    .Build();

host.Run();
