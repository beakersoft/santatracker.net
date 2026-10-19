using System.Runtime.InteropServices;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;
using ILogger = Serilog.ILogger;

namespace SantaTracker.Net.WebApi;

public class Program
{
    public static string DotNetEnvironment { get; private set; } = "Development";

    public static void Main()
    {
        var hostBuilder = CreateHostBuilder().Build();
        hostBuilder.Run();
    }

    public static IHostBuilder CreateHostBuilder()
    {
        DotNetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        HostBuilderContext? hostBuilderContext = null;

        return new HostBuilder()
            .ConfigureAppConfiguration(
                (context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true);

                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddUserSecrets<Program>(optional: true);
                    }

                    builder.AddEnvironmentVariables();

                    hostBuilderContext = context;
                })
            .ConfigureLogging(
                (context, builder) =>
                {
                    // ConfigureAppConfiguration has completed, so User Secrets and
                    // environment variables are available in the final configuration.
                    Log.Logger = CreateLogger(context.HostingEnvironment, context.Configuration);
                    builder.AddSerilog();

                    Log.Information(
                        $"""
                         Starting...
                         Environment:            {context.HostingEnvironment.EnvironmentName}
                         FrameworkDescription:   {RuntimeInformation.FrameworkDescription}
                         OSDescription:          {RuntimeInformation.OSDescription}
                         OSArchitecture:         {RuntimeInformation.OSArchitecture}
                         ProcessArchitecture:    {RuntimeInformation.ProcessArchitecture}
                         """);
                })
            .UseDefaultServiceProvider(
                config =>
                {
                    var validate = hostBuilderContext!.HostingEnvironment.IsDevelopment();
                    config.ValidateScopes = validate;
                    config.ValidateOnBuild = validate;
                })
            .ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseStartup<Startup>();
                });
    }

    /// <summary>
    ///     Setup the logger.
    /// </summary>
    /// <param name="hostContext"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    private static ILogger CreateLogger(IHostEnvironment hostContext, IConfiguration configuration)
    {
        var loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("API", hostContext.ApplicationName)
            .Enrich.WithProperty("env", hostContext.EnvironmentName)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
            .WriteTo.Console()
            .MinimumLevel.Information();

        var grafanaUrl = configuration["Grafana:Url"];
        var grafanaUsername = configuration["Grafana:Username"];
        var grafanaPassword = configuration["Grafana:ApiKey"];

        // Keep local runs working when Grafana credentials have not been configured.
        if (!string.IsNullOrWhiteSpace(grafanaUrl)
            && !string.IsNullOrWhiteSpace(grafanaUsername)
            && !string.IsNullOrWhiteSpace(grafanaPassword))
        {
            loggerConfig.WriteTo.GrafanaLoki(
                grafanaUrl,
                labels: [new() { Key = "app", Value = hostContext.ApplicationName }],
                credentials: new() { Login = grafanaUsername, Password = grafanaPassword });
        }

        return loggerConfig.CreateLogger();
    }
}
