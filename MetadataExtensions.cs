using System.Text.Json;

namespace Homerwarden;

public static class MetadataExtensions
{
    public static Dictionary<string, object?> ExtractMetadata(this string? description)
    {
        if (string.IsNullOrEmpty(description))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object?>>(description) ?? [];
        }
        catch
        {
            // Not JSON, try simple key:value format
            var metadata = new Dictionary<string, object?>();
            var pairs = description.Split(',');

            foreach (var pair in pairs)
            {
                var keyValue = pair.Split(':', 2);
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim();
                    var value = keyValue[1].Trim();

                    metadata[key] = value;
                }
            }

            return metadata;
        }
    }
}
