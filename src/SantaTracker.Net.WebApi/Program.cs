using System.Runtime.InteropServices;
using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace SantaTracker.Net.WebApi;

public class Program
{
    public static string DotNetEnvironment { get; private set; } = "Development";

    public static void Main()
    {
        IHost hostBuilder = CreateHostBuilder().Build();
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
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddEnvironmentVariables();

                    hostBuilderContext = context;

                    Log.Logger = CreateLogger(context.HostingEnvironment);

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
            .ConfigureLogging(
                builder => { builder.AddSerilog(); })
            .UseDefaultServiceProvider(
                config =>
                {
                    bool validate = hostBuilderContext!.HostingEnvironment.IsDevelopment();
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
    /// <returns></returns>
    private static ILogger CreateLogger(IHostEnvironment hostContext)
    {
        LoggerConfiguration loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("API", hostContext.ApplicationName)
            .Enrich.WithProperty("env", hostContext.EnvironmentName)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .WriteTo.Console()
            .MinimumLevel.Information();

        return loggerConfig.CreateLogger();
    }
}
