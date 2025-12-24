using SantaTracker.Net.Application;
using SantaTracker.Net.Application.Features;
using SantaTracker.Net.Infrastructure.Data;
using SantaTracker.Net.WebApi.Extensions;

namespace SantaTracker.Net.WebApi;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services
            .AddLogging()
            .AddMemoryCache()
            .AddApiVersionConfig()
            .AddSwagger()
            .AddOptions()
            .AddHealthChecks();

        services.AddTransient<IGetSantaLocationHandler, GetSantaLocationHandler>();

        services.AddHttpClient<ITrackingDataProvider, FirebaseProvider>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Santa Tracking API v1"); });

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(
            endpoints => { endpoints.MapControllers(); });
    }
}
