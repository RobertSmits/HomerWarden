using System.Text.Json;
using HomerWarden.Models;

namespace Homerwarden;

public static class BookmarkMapper
{
    private static readonly HashSet<string> KnownProperties = new(
        typeof(HomerService).GetProperties().Select(p => p.Name),
        StringComparer.OrdinalIgnoreCase);
    
    public static HomerService ToHomerItem(this LinkwardenBookmark bookmark, Dictionary<string, HomerTag> tags)
    {
        var tagName = ExtractHomerTag(bookmark.Tags);
        var existingTag = tags?.GetValueOrDefault(tagName ?? string.Empty);

        var item = new HomerService
        {
            Name = bookmark.Name,
            Url = bookmark.Url,
            Subtitle = bookmark.ExtensionData.GetValueOrDefault("subtitle")?.ToString(),
            Keywords = bookmark.ExtensionData.GetValueOrDefault("keywords")?.ToString(),
            Logo = bookmark.ExtensionData.GetValueOrDefault("logo")?.ToString(),
            Target = bookmark.ExtensionData.GetValueOrDefault("target")?.ToString(),
            Quick = bookmark.ExtensionData.GetValueOrDefault("quick") as HomerLink[],
            Tag = existingTag?.Tag,
            Tagstyle = existingTag?.TagStyle,
        };

        if (bookmark.ExtensionData is not null)
        {
            item.ExtensionData = bookmark.ExtensionData
                .Where(kvp => !IsKnownProperty(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => JsonSerializer.SerializeToElement(kvp.Value));
        }

        return item;
    }

    private static bool IsKnownProperty(string key) => KnownProperties.Contains(key);

    private static string? ExtractHomerTag(List<LinkwardenTag> tags)
    {
        return tags
            .FirstOrDefault(t => t.Name.StartsWith("homer:", StringComparison.OrdinalIgnoreCase))
            ?.Name.Replace("homer:", string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}