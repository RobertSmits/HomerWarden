namespace HomerWarden.Services;

public sealed class SyncHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SyncHostedService> _logger;
    private readonly int _syncIntervalMinutes;

    public SyncHostedService(IServiceProvider serviceProvider, ILogger<SyncHostedService> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _syncIntervalMinutes = int.Parse(configuration["SYNC_INTERVAL_MINUTES"] ?? "15");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SyncHostedService started. Sync interval: {Interval} minutes", _syncIntervalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var syncService = scope.ServiceProvider.GetRequiredService<SyncService>();
                    await syncService.SyncAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during scheduled sync");
            }

            await Task.Delay(TimeSpan.FromMinutes(_syncIntervalMinutes), stoppingToken);
        }

        _logger.LogInformation("SyncHostedService stopped");
    }
}
