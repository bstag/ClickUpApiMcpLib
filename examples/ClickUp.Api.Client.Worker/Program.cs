using ClickUp.Api.Client.Worker;
using Serilog;

public class Program
{
    public static void Main(string[] args)
    {
        // Configure Serilog for initial bootstrap logging
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console() // Basic console sink for bootstrap
            .CreateBootstrapLogger(); // Use CreateBootstrapLogger for early logging

        try
        {
            Log.Information("Worker Service Starting Up...");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Worker Service terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration) // Reads from appsettings.json
                .ReadFrom.Services(services) // Allows services to influence logging
                .Enrich.FromLogContext()
                .WriteTo.Console()) // Configures console sink via appsettings or here
            .ConfigureServices((hostContext, services) =>
            {
                // ClickUpApiClientOptions will be read from IConfiguration (appsettings.json, user secrets)
                // and bound by AddClickUpApiClient extension in a later step.

                // Worker service registration
                services.AddHostedService<Worker>();
            });
}
