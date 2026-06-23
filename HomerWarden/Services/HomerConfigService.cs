using Homerwarden;
using HomerWarden.Models;
using System.Text.Json;

namespace HomerWarden.Services;

public sealed class HomerConfigService
{
    private readonly ILogger<HomerConfigService> _logger;
    private readonly IConfiguration _configuration;

    public HomerConfigService(ILogger<HomerConfigService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<HomerConfig> CreateConfigFromCollectionAsync(HomerConfig existingConfig, LinkwardenCollection collection)
    {
        var links = new List<HomerLink>();
        var services = new List<HomerCategory>();

        // Process root-level bookmarks as links
        foreach (var bookmark in collection.Bookmarks.OrderBy(b => GetSortOrder(b.ExtensionData)))
        {
            var link = new HomerLink
            {
                Name = bookmark.Name,
                Url = bookmark.Url,
                Icon = bookmark.ExtensionData.GetValueOrDefault("icon").ToString(),
                Target = bookmark.ExtensionData.GetStringOrDefault("target", "_blank"),
            };
            links.Add(link);
        }

        // Process sub-collections as services (categories)
        foreach (var subCollection in collection.Children.OrderBy(c => GetSortOrder(c.ExtensionData)))
        {
            var service = new HomerCategory
            {
                Name = subCollection.Name,
                Icon = subCollection.ExtensionData.GetValueOrDefault("icon").ToString(),
                Items = [],
            };

            // Process bookmarks as items
            foreach (var bookmark in subCollection.Bookmarks.OrderBy(b => GetSortOrder(b.ExtensionData)))
            {
                var item = bookmark.ToHomerItem(existingConfig.Tags ?? []);
                service.Items.Add(item);
            }

            if (service.Items.Any())
            {
                services.Add(service);
            }
        }

        return existingConfig with { Links = links, Services = services };
    }

    public async Task SaveConfigAsync(string configName, HomerConfig config)
    {
        try
        {
            var homerPath = _configuration["HOMER_MOUNT_PATH"];
            var filePath = Path.Combine(homerPath, $"{configName}.yml");

            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? homerPath);

            using var jsonStream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(jsonStream, config, AppJsonSerializerContext.Default.HomerConfig);

            _logger.LogInformation("Saved Homer config to {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving Homer config {ConfigName}", configName);
            throw;
        }
    }

    public async Task<HomerConfig> LoadConfigAsync(string configName)
    {
        var homerPath = _configuration["HOMER_MOUNT_PATH"];
        var filePath = Path.Combine(homerPath, $"{configName}.yml");

        if (!File.Exists(filePath))
        {
            return new HomerConfig();
        }
       
        using var jsonStream = File.OpenRead(filePath);
        
        try
        {
            var config = JsonSerializer.Deserialize(jsonStream, AppJsonSerializerContext.Default.HomerConfig);
            return config ?? new HomerConfig();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize existing Homer config {ConfigName}, returning empty config", configName);
            return new HomerConfig();
        }
    }

    private static int GetSortOrder(Dictionary<string, JsonElement> metadata)
    {
        var orderValue = metadata.GetValueOrDefault("order");
        return orderValue.ValueKind switch
        {
            JsonValueKind.Number => orderValue.GetInt32(),
            JsonValueKind.String when int.TryParse(orderValue.GetString(), out var parsedOrder) => parsedOrder,
            _ => int.MaxValue
        };
    }
}
