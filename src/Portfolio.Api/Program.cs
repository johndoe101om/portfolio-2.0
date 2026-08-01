using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Services;
using Portfolio.Application.Interfaces;
using Portfolio.Infrastructure.Email;
using Portfolio.Infrastructure.Repositories;
using Portfolio.Api.Middleware;
using Portfolio.Api.HealthChecks;

LoadLocalEnvFile();

var builder = WebApplication.CreateBuilder(args);

// ── JSON serialisation ────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ── Database ──────────────────────────────────────────────────────────────────
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<PortfolioDbContext>(opts =>
    opts.UseNpgsql(connStr, npgsql =>
    {
        npgsql.MigrationsAssembly("Portfolio.Infrastructure");
        npgsql.EnableRetryOnFailure(3);
    })
    .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

// ── Application services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IStorageService, SupabaseStorageService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddSingleton<IAdminAuthService, AdminAuthService>();
builder.Services.AddHostedService<HeartbeatService>();

// ── HTTP Clients ──────────────────────────────────────────────────────────────
builder.Services.AddHttpClient();

// ── CORS ──────────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:5173", "https://www.codersatyam.com"];

builder.Services.AddCors(opts => opts.AddPolicy("PortfolioPolicy", policy =>
    policy.WithOrigins(allowedOrigins)
          .AllowAnyHeader()
          .AllowAnyMethod()
          .WithExposedHeaders("X-Pagination")));

// ── Rate limiting ─────────────────────────────────────────────────────────────
builder.Services.AddRateLimiter(opts =>
{
    // General API rate limit: 100 req/min per IP
    opts.AddFixedWindowLimiter("general", o =>
    {
        o.PermitLimit = 100;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 5;
    });

    // Strict contact form limit: 5 submissions per 10 min per IP
    opts.AddFixedWindowLimiter("contact", o =>
    {
        o.PermitLimit = 5;
        o.Window = TimeSpan.FromMinutes(10);
        o.QueueLimit = 0;
    });

    opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ── OpenAPI / Swagger ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Satyam Portfolio API",
        Version = "v1",
        Description = "REST API for the Satyam Kumar portfolio site.",
        Contact = new OpenApiContact { Name = "Satyam Kumar", Email = "sirsatyamchaudhary@gmail.com" },
    });
});

// ── Health checks ─────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

// ── Problem Details (RFC 7807) ────────────────────────────────────────────────
builder.Services.AddProblemDetails();

// ── Logging ───────────────────────────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
if (!builder.Environment.IsDevelopment())
    builder.Logging.AddJsonConsole();

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();
// ─────────────────────────────────────────────────────────────────────────────

// ── Database migration on startup ─────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        if (db.Database.IsRelational())
        {
            await db.Database.MigrateAsync();
            await SyncPostgresIdentitySequencesAsync(db, logger);
        }
        else
        {
            await db.Database.EnsureCreatedAsync();
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Migration warning during startup. Ensured database creation.");
        await db.Database.EnsureCreatedAsync();
        if (db.Database.IsRelational())
            await SyncPostgresIdentitySequencesAsync(db, logger);
    }
}

// ── Security headers ──────────────────────────────────────────────────────────
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    ctx.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
    if (!app.Environment.IsDevelopment())
        ctx.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    await next();
});

// ── Request size limit ────────────────────────────────────────────────────────
app.Use(async (ctx, next) =>
{
    ctx.Request.EnableBuffering();
    var maxLimit = ctx.Request.Path.StartsWithSegments("/api/projects/upload-image") ? 10_485_760 : 2_097_152;
    if (ctx.Request.ContentLength > maxLimit)
    {
        ctx.Response.StatusCode = 413;
        await ctx.Response.WriteAsync("Request too large.");
        return;
    }
    await next();
});

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("PortfolioPolicy");
app.UseRateLimiter();

// ── Request logging middleware ────────────────────────────────────────────────
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();
app.MapControllers();

// ── Root status endpoint ──────────────────────────────────────────────────────
app.MapGet("/", () => Results.Ok(new
{
    name = "Portfolio API",
    status = "Online",
    version = "1.0.0",
    docs = "/swagger",
    health = "/health"
}));

// ── Health check endpoint ─────────────────────────────────────────────────────
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new { name = e.Key, status = e.Value.Status.ToString() }),
            totalDuration = report.TotalDuration,
        };
        await ctx.Response.WriteAsJsonAsync(result);
    },
});

await app.RunAsync();

static void LoadLocalEnvFile()
{
    foreach (var path in GetLocalEnvFileCandidates().Distinct(StringComparer.OrdinalIgnoreCase))
    {
        if (!File.Exists(path))
            continue;

        foreach (var rawLine in File.ReadAllLines(path))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
                continue;

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
                continue;

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();

            if (value.Length >= 2 &&
                ((value[0] == '"' && value[^1] == '"') || (value[0] == '\'' && value[^1] == '\'')))
            {
                value = value[1..^1];
            }

            if (!string.IsNullOrWhiteSpace(key) && Environment.GetEnvironmentVariable(key) is null)
                Environment.SetEnvironmentVariable(key, value);
        }
    }
}

static IEnumerable<string> GetLocalEnvFileCandidates()
{
    var currentDirectory = Directory.GetCurrentDirectory();

    yield return Path.Combine(currentDirectory, "src", "Portfolio.Api", ".env");
    yield return Path.Combine(currentDirectory, ".env");
    yield return Path.Combine(AppContext.BaseDirectory, ".env");
}

static async Task SyncPostgresIdentitySequencesAsync(PortfolioDbContext db, ILogger logger)
{
    if (!string.Equals(db.Database.ProviderName, "Npgsql.EntityFrameworkCore.PostgreSQL", StringComparison.Ordinal))
        return;

    var identityTables = new[]
    {
        "AuditLogs",
        "BlogPosts",
        "Categories",
        "ContactMessages",
        "Educations",
        "Experiences",
        "Profiles",
        "ProjectAchievements",
        "ProjectCategories",
        "ProjectFeatures",
        "ProjectImages",
        "ProjectLinks",
        "Projects",
        "ProjectSkills",
        "ProjectTechnologies",
        "Services",
        "SiteSettings",
        "Skills",
        "SocialLinks",
        "Statistics",
        "Technologies",
        "Testimonials"
    };

    foreach (var tableName in identityTables)
    {
        try
        {
            var sql = $"""
SELECT setval(
    pg_get_serial_sequence('"{tableName}"', 'Id'),
    GREATEST(COALESCE((SELECT MAX("Id") FROM "{tableName}"), 0), 1),
    COALESCE((SELECT MAX("Id") FROM "{tableName}"), 0) > 0
);
""";
            await db.Database.ExecuteSqlRawAsync(sql);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not synchronize identity sequence for {TableName}.", tableName);
        }
    }
}

// Expose for integration tests
public partial class Program { }
