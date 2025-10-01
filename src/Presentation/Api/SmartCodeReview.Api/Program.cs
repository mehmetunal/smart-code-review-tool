using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using FluentValidation;
using SmartCodeReview.Api.BackgroundServices;
using SmartCodeReview.Api.Extensions;
using SmartCodeReview.Api.Middleware;
using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Dto.Mssql.Validators;
using SmartCodeReview.Mssql;
using SmartCodeReview.Mssql.Services.Interfaces;
using SmartCodeReview.Mssql.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog konfigürasyonu
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

// Redis connection
builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379";
    return StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnection);
});

// Service registrations
builder.Services.AddScoped<ICodeReviewService, CodeReviewService>();
builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddSingleton<IWebhookQueueService, WebhookQueueService>();
builder.Services.AddScoped<IAIService, OllamaAIService>();
builder.Services.AddScoped<IApiConfigurationService, ApiConfigurationService>();

// Background services
builder.Services.AddHostedService<WebhookProcessorWorker>();

// Database connection
builder.Services.AddDbContext<SmartCodeReviewDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity configuration
builder.Services.AddIdentity<User, Role>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<SmartCodeReviewDbContext>()
.AddDefaultTokenProviders();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware'ler
app.UseGlobalExceptionHandler();
app.UseRequestLogging();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
}));

Log.Information("SmartCodeReview API başlatılıyor...");

// Seed data oluştur
await app.SeedDataAsync();

app.Run();

