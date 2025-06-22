using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Set up configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Set up Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Services will be configured in the next step
                // For now, this sets up the basic host
            })
            .UseSerilog()
            .Build();

        Log.Information("Console Example Application Starting...");

        // Application logic will go here in later steps.
        // For now, just demonstrate the host is running.
        Console.WriteLine("Hello, World! Configuration and Logging initialized.");
        Console.WriteLine("Check appsettings.json and appsettings.template.json for API token configuration.");

        // Example of how to get a service if needed directly (usually resolve from DI scope)
        // var myService = host.Services.GetRequiredService<IMyService>();
        // await myService.DoSomethingAsync();

        await host.RunAsync(); // Or use specific services without full host run for simpler console apps

        Log.Information("Console Example Application Shutting Down...");
    }
}
