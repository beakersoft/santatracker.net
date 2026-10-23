using System.Reflection;
using Asp.Versioning;
using Microsoft.OpenApi;

namespace SantaTracker.Net.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        return services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Title = "Santa Tracking API",
                            Version = "v1",
                            Description = """
        A free, public API for tracking Santa’s journey on Christmas Eve.

        The main endpoint is cached for one minute, so displays should refresh at most once per minute.

        Visit [SleighWatch](https://sleighwatch.com) for integration examples and guides.
        """
                        });
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                    c.SchemaFilter<SantaLocationResponseSchemaFilter>();
                });
    }

    public static IServiceCollection AddApiVersionConfig(this IServiceCollection services)
    {
        _ = services.AddApiVersioning(
                options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
                    options.ReportApiVersions = true;
                })
            .AddApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.AssumeDefaultVersionWhenUnspecified = true;
                });

        return services;
    }
}
