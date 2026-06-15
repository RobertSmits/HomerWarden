using HomerWarden.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<LinkwardenService>();
builder.Services.AddScoped<HomerConfigService>();
builder.Services.AddScoped<SyncService>();
builder.Services.AddHostedService<SyncHostedService>();
builder.Services.AddLogging();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("Health")
    .WithOpenApi();

app.MapPost("/sync", async (SyncService syncService, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Manual sync triggered");
        await syncService.SyncAsync();
        return Results.Ok(new { message = "Sync completed successfully" });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Sync failed");
        return Results.StatusCode(500);
    }
})
.WithName("TriggerSync")
.WithOpenApi();

app.Run();
