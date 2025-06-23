using ClickUp.Api.Client.Worker;
using Serilog;
using ClickUp.Api.Client.Extensions;

// Helper class for worker settings
public class WorkerExampleSettings
{
    public string? ListIdForPolling { get; set; }
    public int PollingIntervalSeconds { get; set; } = 60;
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Worker Service Starting Up...");
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
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
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console())
            .ConfigureServices((hostContext, services) =>
            {
                services.AddClickUpClient(options =>
                {
                    hostContext.Configuration.GetSection("ClickUpApiOptions").Bind(options);
                    if (string.IsNullOrEmpty(options.PersonalAccessToken)) // Removed OAuthToken check
                    {
                        Log.Warning("ClickUp API PersonalAccessToken not configured. Please check appsettings.json or user secrets.");
                    }
                });

                services.Configure<WorkerExampleSettings>(hostContext.Configuration.GetSection("WorkerExampleSettings"));

                services.AddHostedService<Worker>();
            });
}
