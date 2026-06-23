using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomerWarden.Models;

public record HomerConfig
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; init; }

    [JsonPropertyName("logo")]
    public string? Logo { get; init; }

    [JsonPropertyName("header")]
    public bool Header { get; init; } = true;

    [JsonPropertyName("footer")]
    public string Footer { get; init; } = string.Empty;

    [JsonPropertyName("columns")]
    public int Columns { get; init; } = 3;

    [JsonPropertyName("connectivityCheck")]
    public bool ConnectivityCheck { get; init; } = true;

    [JsonPropertyName("theme")]
    public string Theme { get; init; } = "default";

    [JsonPropertyName("stylesheet")]
    public string[]? Stylesheet { get; init; }

    [JsonPropertyName("tags")]
    public Dictionary<string, HomerTag>? Tags { get; init; }

    [JsonPropertyName("links")]
    public List<HomerLink> Links { get; init; } = [];

    [JsonPropertyName("updateIntervalMs")]
    public int UpdateIntervalMs { get; init; } = 30000;

    [JsonPropertyName("services")]
    public List<HomerCategory> Services { get; init; } = [];
}

public record HomerTag
{
    [JsonPropertyName("tag")]
    public required string Tag { get; init; }

    [JsonPropertyName("tagstyle")]
    public required string TagStyle { get; init; }
}

public record HomerCategory
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("icon")]
    public string? Icon { get; init; }

    [JsonPropertyName("items")]
    public List<HomerService> Items { get; init; } = [];
}

public record HomerService
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; init; }

    [JsonPropertyName("keywords")]
    public string? Keywords { get; init; }

    [JsonPropertyName("logo")]
    public string Logo { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    [JsonPropertyName("target")]
    public string Target { get; init; } = "_blank";

    [JsonPropertyName("quick")]
    public HomerLink[]? Quick { get; init; }

    [JsonPropertyName("tag")]
    public string? Tag { get; init; }

    [JsonPropertyName("tagstyle")]
    public string? TagStyle { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public record HomerLink
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("icon")]
    public string? Icon { get; init; }

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    [JsonPropertyName("target")]
    public string Target { get; init; } = "_blank";
}

