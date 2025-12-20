using System.Runtime.InteropServices;
using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace SantaTracker.Net
{
    public class Program
    {
        public static string DotNetEnvironment { get; private set; }

        public static void Main(string[] args)
        {
            var hostBuilder = CreateHostBuilder().Build();
            hostBuilder.Run();

            //var builder = WebApplication.CreateBuilder(args);

            //// Add services to the container.

            //builder.Services.AddControllers();
            //// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            //var app = builder.Build();

            //// Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            //app.UseHttpsRedirection();

            //app.UseAuthorization();

            //app.MapControllers();

            //app.Run();
        }

        public static IHostBuilder CreateHostBuilder()
        {
            DotNetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            HostBuilderContext hostBuilderContext = null;

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
                        var validate = hostBuilderContext.HostingEnvironment.IsDevelopment();
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
            var loggerConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("API", hostContext.ApplicationName)
                .Enrich.WithProperty("env", hostContext.EnvironmentName)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Information();

            return loggerConfig.CreateLogger();
        }
    }
}