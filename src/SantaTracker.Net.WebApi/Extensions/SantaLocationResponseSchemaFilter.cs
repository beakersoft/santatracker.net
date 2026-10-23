using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using SantaTracker.Net.Contracts.Responses;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SantaTracker.Net.WebApi.Extensions;

/// <summary>
/// Provides a realistic response example in the generated OpenAPI document.
/// </summary>
public sealed class SantaLocationResponseSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(GetSantaLocationResponse))
        {
            return;
        }

        if (schema is not OpenApiSchema santaLocationSchema)
        {
            return;
        }

        santaLocationSchema.Example = new JsonObject
        {
            ["description"] = "Delivering presents in London, United Kingdom",
            ["fromCity"] = "London",
            ["toCity"] = string.Empty,
            ["country"] = "United Kingdom",
            ["status"] = "InFlight"
        };
    }
}
