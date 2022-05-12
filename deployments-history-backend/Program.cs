using DeploymentsHistoryBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace DeploymentsHistoryBackend;

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
        //CreateHostBuilder(args).Build().Run();
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
            options.UseSqlServer(builder.Configuration.GetConnectionString("DeploymentsHistory")));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add services to the container.
        builder.Services.RegisterServices();
        builder.Services.RegisterConfiguration(builder.Configuration);

        builder.Services.AddCustomCors();
        builder.Services.AddMemoryCache(o =>
        {
            o.SizeLimit = 256;
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
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}