using System.Text;
using AspNetCore.Localizer.Json.Extensions;
using AspNetCore.Localizer.Json.JsonOptions;
using Microsoft.AspNetCore.Localization;
using SantaTracker.Net.Application;
using SantaTracker.Net.Application.Features;
using SantaTracker.Net.Contracts.Constants;
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

        var serviceCollection = services.AddJsonLocalization(options =>
        {
            options.ResourcesPath = "Localizations";
            options.LocalizationMode = LocalizationMode.I18n;
            options.CacheDuration = TimeSpan.FromMinutes(30);
            options.FileEncoding = Encoding.UTF8;
            options.UseEmbeddedResources = false;
            options.SupportedCultureInfos = SupportedCultures.CulturesHashSet;
        });

        services.AddTransient<IGetSantaLocationHandler, GetSantaLocationHandler>();
        services.AddTransient<ITrackingDataProvider, LocalJsonProvider>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        // Configure the pipeline middleware to catch 'Accept-Language' headers and query string parameters for localization
        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(SupportedCultures.Cultures[0]),
            SupportedCultures = [.. SupportedCultures.CulturesHashSet],
            SupportedUICultures = [.. SupportedCultures.CulturesHashSet]
        };
        localizationOptions.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider
        {
            QueryStringKey = "lang",
            UIQueryStringKey = "lang"
        });
        app.UseRequestLocalization(localizationOptions);

        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Santa Tracking API v1"); });

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(
            endpoints => { endpoints.MapControllers(); });
    }
}
