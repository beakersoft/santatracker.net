using System.Reflection;
using Asp.Versioning;
using Microsoft.OpenApi;

namespace SantaTracker.Net.WebApi.Extensions
{
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
                                Version = "v1"
                            });
                        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                        c.IncludeXmlComments(xmlPath);
                    });
        }

        public static IServiceCollection AddApiVersionConfig(this IServiceCollection services)
        {
            services.AddApiVersioning(
                    options =>
                    {
                        options.DefaultApiVersion = new ApiVersion(1, 0);
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
}