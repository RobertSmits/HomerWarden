namespace HomerWarden.Services;

public sealed class SyncService
{
    private readonly LinkwardenService _linkwardenService;
    private readonly HomerConfigService _homerConfigService;
    private readonly ILogger<SyncService> _logger;

    public SyncService(
        LinkwardenService linkwardenService,
        HomerConfigService homerConfigService,
        ILogger<SyncService> logger)
    {
        _linkwardenService = linkwardenService;
        _homerConfigService = homerConfigService;
        _logger = logger;
    }

    public async Task SyncAsync()
    {
        try
        {
            _logger.LogInformation("Starting sync from Linkwarden to Homer");

            // Get all homer: prefixed collections from Linkwarden
            var collections = await _linkwardenService.GetHomerCollectionsAsync();
            _logger.LogInformation("Found {Count} Homer collections", collections.Count);

            foreach (var collection in collections)
            {
                // Extract config name from collection name (e.g., "homer:config" -> "config")
                var configName = collection.Name.Replace("homer:", "");

                // Fetch full collection hierarchy with bookmarks
                var fullCollection = await _linkwardenService.GetCollectionWithChildrenAsync(collection);
                if (fullCollection is null)
                {
                    _logger.LogWarning("Failed to fetch collection {CollectionName}", collection.Name);
                    continue;
                }

                var existingConfig = await _homerConfigService.LoadConfigAsync(configName);
                var newConfig = await _homerConfigService.UpdateConfigFromCollectionAsync(existingConfig, fullCollection);
                
                // Save merged config
                await _homerConfigService.SaveConfigAsync(configName, existingConfig);
                _logger.LogInformation("Synced {ConfigName}.yml with {ServiceCount} categories", configName, existingConfig.Services.Count);
            }

            _logger.LogInformation("Sync completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sync");
            throw;
        }
    }
}
