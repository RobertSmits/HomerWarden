using System.Diagnostics.CodeAnalysis;
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
        var quick = bookmark.ExtensionData.GetValueOrDefault("quick");

        var item = new HomerService
        {
            Name = bookmark.Name,
            Url = bookmark.Url,
            Subtitle = bookmark.ExtensionData.GetStringOrDefault("subtitle"),
            Keywords = bookmark.ExtensionData.GetStringOrDefault("keywords"),
            Logo = bookmark.ExtensionData.GetStringOrDefault("logo")!,
            Target = bookmark.ExtensionData.GetStringOrDefault("target", "_blank"),
            Quick = quick.ValueKind is JsonValueKind.Array 
                ? JsonSerializer.Deserialize<HomerLink[]>(quick)
                : [],
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

    [return: NotNullIfNotNull(nameof(fallback))]
    public static string? GetStringOrDefault(this Dictionary<string, JsonElement> data, string key, string? fallback = default)
    {
        if (data != null && data.TryGetValue(key, out var element) 
            && element.ValueKind != JsonValueKind.Undefined 
            && element.ValueKind != JsonValueKind.Null)
        {
            return element.GetString();
        }

        return fallback;
    }

    private static string? ExtractHomerTag(List<LinkwardenTag> tags)
    {
        return tags
            .FirstOrDefault(t => t.Name.StartsWith("homer:", StringComparison.OrdinalIgnoreCase))
            ?.Name.Replace("homer:", string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}