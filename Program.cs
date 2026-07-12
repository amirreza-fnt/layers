using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using LayerManager.API.Data;
using LayerManager.API.Middleware;
using LayerManager.API.Models;
using LayerManager.API.Repositories;
using LayerManager.API.Repositories.Interfaces;
using LayerManager.API.Services;
using LayerManager.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LayerManager API",
        Version = "v1",
        Description = "API مدیریت لایه‌های نقشه سبزوار",
        Contact = new OpenApiContact
        {
            Name = "LayerManager Team"
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=.;Database=LayerManagerDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
        sqlOptions.CommandTimeout(30);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? new[] { "http://localhost:3000", "https://map.sabzevar.ir" };

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });

    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
    }
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults.MimeTypes
        .Concat(new[] { "application/json" });
});

builder.Services.AddScoped<IMapLayerRepository, MapLayerRepository>();
builder.Services.AddScoped<IMapLayerService, MapLayerService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseResponseCompression();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "LayerManager API v1");
        options.RoutePrefix = "swagger";
    });
    app.UseCors("AllowAll");
}
else
{
    app.UseCors("AllowFrontend");
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var created = context.Database.EnsureCreated();
        if (created)
        {
            logger.LogInformation("Database created successfully!");
            await SeedDataAsync(context, logger);
        }
        logger.LogInformation("Database ready");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization error");
    }
}

app.Logger.LogInformation("\n══════════════════════════════════════");
app.Logger.LogInformation("LayerManager API Started");
app.Logger.LogInformation("Environment: {Env}", app.Environment.EnvironmentName);
app.Logger.LogInformation("Swagger: /swagger");
app.Logger.LogInformation("══════════════════════════════════════\n");

app.Run();

static async Task SeedDataAsync(AppDbContext context, ILogger logger)
{
    if (await context.MapLayers.AnyAsync()) return;

    var layers = new List<MapLayer>
    {
        new()
        {
            Slug = "toll",
            Name = "پرداخت عوارض",
            Description = "مشاهده و پرداخت عوارض نوسازی و پسماند",
            IconName = "CreditCard",
            Color = "#3B82F6",
            SortOrder = 1,
            IsActive = true,
            ComponentName = "NosaziLayer"
        },
        new()
        {
            Slug = "kooche",
            Name = "کوچه‌ها",
            Description = "نمایش کوچه‌ها و معابر شهر سبزوار",
            IconName = "Route",
            Color = "#10B981",
            SortOrder = 2,
            IsActive = false,
            ComponentName = "KoocheLayer"
        },
        new()
        {
            Slug = "mokeb",
            Name = "موکب نما",
            Description = "لیست موقعیت موکب و اسکان‌های شهر سبزوار",
            IconName = "MapPin",
            Color = "#8B5CF6",
            SortOrder = 3,
            IsActive = true,
            ComponentName = "MokebLayer"
        }
    };

    context.MapLayers.AddRange(layers);
    await context.SaveChangesAsync();
    logger.LogInformation("Seed data inserted: {Count} layers", layers.Count);
}

public partial class Program { }
