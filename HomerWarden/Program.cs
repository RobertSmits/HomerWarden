using System.Text.Json;
using System.Text.Json.Serialization;
using HomerWarden.Models;
using HomerWarden.Services;

var builder = WebApplication.CreateSlimBuilder(args);

// Add services
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<LinkwardenService>();
builder.Services.AddScoped<HomerConfigService>();
builder.Services.AddScoped<SyncService>();
builder.Services.AddHostedService<SyncHostedService>();
builder.Services.AddLogging();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new HealthResponse("healthy", DateTime.UtcNow)))
    .WithName("Health");

app.MapPost("/sync", async (SyncService syncService, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Manual sync triggered");
        await syncService.SyncAsync();
        return Results.Ok(new MessageResponse("Sync completed successfully"));
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Sync failed");
        return Results.StatusCode(500);
    }
})
.WithName("TriggerSync");

app.Run();

public record MessageResponse(string Message);
public record HealthResponse(string Status, DateTime Timestamp);

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(MessageResponse))]
[JsonSerializable(typeof(HealthResponse))]
[JsonSerializable(typeof(HomerConfig))]
[JsonSerializable(typeof(HomerLink[]))]
[JsonSerializable(typeof(LinkwardenApiResponse<LinkwardenCollection>))]
[JsonSerializable(typeof(LinkwardenApiData<LinkwardenPagedLinks>))]
[JsonSerializable(typeof(LinkwardenPagedLinks))]
[JsonSerializable(typeof(LinkwardenCollection))]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}