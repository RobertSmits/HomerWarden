using System.Text.Json;

namespace Homerwarden;

public static class MetadataExtensions
{
    public static Dictionary<string, JsonElement> ExtractMetadata(this string? description)
    {
        if (string.IsNullOrEmpty(description))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize(description, AppJsonSerializerContext.Default.DictionaryStringJsonElement) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
