using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomerWarden.Models;

public record HomerConfig
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }

    [JsonPropertyName("logo")]
    public string? Logo { get; set; }

    [JsonPropertyName("header")]
    public bool Header { get; set; } = true;

    [JsonPropertyName("footer")]
    public string Footer { get; set; } = string.Empty;

    [JsonPropertyName("columns")]
    public int Columns { get; set; } = 3;

    [JsonPropertyName("connectivityCheck")]
    public bool ConnectivityCheck { get; set; } = true;

    [JsonPropertyName("theme")]
    public string Theme { get; set; } = "default";

    [JsonPropertyName("stylesheet")]
    public string[]? Stylesheet { get; set; }

    [JsonPropertyName("tags")]
    public Dictionary<string, HomerTag>? Tags { get; set; }

    [JsonPropertyName("links")]
    public List<HomerLink> Links { get; set; } = [];

    [JsonPropertyName("updateIntervalMs")]
    public int UpdateIntervalMs { get; set; } = 30000;

    [JsonPropertyName("services")]
    public List<HomerCategory> Services { get; set; } = [];
}

public record HomerTag
{
    [JsonPropertyName("tag")]
    public required string Tag { get; set; }

    [JsonPropertyName("tagstyle")]
    public required string TagStyle { get; set; }
}

public record HomerCategory
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("items")]
    public List<HomerService> Items { get; set; } = [];
}

public record HomerService
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }

    [JsonPropertyName("keywords")]
    public string? Keywords { get; set; }

    [JsonPropertyName("logo")]
    public string Logo { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("target")]
    public string Target { get; set; } = "_blank";

    [JsonPropertyName("quick")]
    public HomerLink[]? Quick { get; set; }

    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

    [JsonPropertyName("tagstyle")]
    public string? Tagstyle { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public record HomerLink
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("target")]
    public string Target { get; set; } = "_blank";
}

