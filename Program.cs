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
        Description = "API مدیریت دسته‌بندی‌ها و راهنمای نقشه سبزوار",
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

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IGuideRepository, GuideRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGuideService, GuideService>();

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
            logger.LogInformation("New tables created (CategoryHierarchies, MapGuides)");
        }

        var categoryService = services.GetRequiredService<ICategoryService>();
        await categoryService.SyncCategoriesAsync();
        logger.LogInformation("Categories synced successfully");

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

public partial class Program { }
