using System.Text.Json;
using System.Text.Json.Serialization;
using Homerwarden;

namespace HomerWarden.Models;

public record LinkwardenCollection
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int? ParentId { get; init; }
    
    [JsonIgnore]
    public List<LinkwardenCollection> Children { get; init; } = [];

    [JsonIgnore]
    public List<LinkwardenBookmark> Bookmarks { get; init; } = [];

    public Dictionary<string, JsonElement> ExtensionData
    {
        get => field ??= Description.ExtractMetadata();
        set;
    }
}

public record LinkwardenBookmark
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int CollectionId { get; init; }
    public List<LinkwardenTag> Tags { get; init; } = [];

    public Dictionary<string, JsonElement> ExtensionData { 
        get => field ??= Description.ExtractMetadata();
        init;
    }
}

public record LinkwardenTag
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public record LinkwardenPagedLinks
{
    public List<LinkwardenBookmark> Links { get; init; } = [];
    public int? NextCursor { get; init; }
}

public record LinkwardenApiData<T>
{
    public T Data { get; init; } = default!;
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty; 
}

public record LinkwardenApiResponse<T>
{
    public List<T> Response { get; init; } = [];
}
