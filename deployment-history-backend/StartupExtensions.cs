using DeploymentHistoryBackend.Data;
using DeploymentHistoryBackend.Models.Config;
using DeploymentHistoryBackend.Services;

namespace DeploymentHistoryBackend
{
    public static class StartupExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<HttpClient>();
            services.AddHttpClient();
            services.AddTransient<IBitbucketRepository, BitbucketRepository>();
            services.AddTransient<IReleasesService, ReleasesService>();
            services.AddTransient<ICachedReleasesService, CachedReleasesService>();
            services.AddTransient<IApplicationsRepository, ApplicationsRepository>();
            services.AddTransient<ICachedApplicationsRepository, CachedApplicationsRepository>();
            services.AddTransient<IDeploymentsRepository, DeploymentsRepository>();
            services.AddTransient<IDeploymentsService, DeploymentsService>();
        }

        public static void RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BitbucketConfig>(options => configuration.GetSection("BitbucketConfig").Bind(options));
        }

        private const string ProdOrigin = "prodOrigin";
        private const string AllOrigins = "allOrigins";

        public static void AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(ProdOrigin, builder =>
                {
                    builder.WithOrigins("");
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowCredentials();
                });

                options.AddPolicy(AllOrigins, builder =>
                {
                    builder.WithOrigins("http://localhost:3000");
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowCredentials();
                });
            });
        }

        public static void UseCustomCors(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseCors(AllOrigins);
            }
            if (env.EnvironmentName == "qa")
            {
                app.UseCors(ProdOrigin);
            }
        }
    }
}
