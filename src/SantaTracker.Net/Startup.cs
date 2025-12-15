using SantaTracker.Net.Application.Features;
using SantaTracker.Net.Extensions;

namespace SantaTracker.Net
{
    public class Startup(IConfiguration configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services
                .AddLogging()
                .AddApiVersionConfig()
                .AddSwagger()
                .AddOptions()
                .AddHealthChecks();

            services.AddTransient<IGetSantaLocationHandler, GetSantaLocationHandler>();
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
}