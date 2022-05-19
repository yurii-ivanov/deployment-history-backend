using DeploymentHistoryBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace DeploymentHistoryBackend;

public class Program
{
    public IConfiguration Configuration { get; set; }
    public static TimeZoneInfo? AppTimeZone;
    public Program(IConfiguration configuration)
    {
        Configuration = configuration;
    }


    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder);
        var app = builder.Build();
        Configure(app, builder.Environment);
    }

    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        AppTimeZone = TimeZoneInfo.FindSystemTimeZoneById(builder.Configuration["Timezone"]);

        builder.Services.AddControllers()
            .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

        builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DeploymentHistory")));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add services to the container.
        builder.Services.RegisterServices();
        builder.Services.RegisterConfiguration(builder.Configuration);

        builder.Services.AddCustomCors();
        builder.Services.AddMemoryCache(o =>
        {
            o.SizeLimit = 512;
            o.CompactionPercentage = 0.25;
        });
    }

    public static void Configure(WebApplication app, IWebHostEnvironment env)
    {

        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCustomCors(env);

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}