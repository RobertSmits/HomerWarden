using Homerwarden;

namespace HomerWarden.Models;

public class LinkwardenCollection
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public List<LinkwardenCollection> Children { get; set; } = [];
    public List<LinkwardenBookmark> Bookmarks { get; set; } = [];
    
    public Dictionary<string, object?> ExtensionData { 
        get => field ??= Description.ExtractMetadata();
        set => field = value ??= [];
     }
}

public class LinkwardenBookmark
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CollectionId { get; set; }
    public List<LinkwardenTag> Tags { get; set; } = [];

    public Dictionary<string, object?> ExtensionData { 
        get => field ??= Description.ExtractMetadata();
        set;
    }
}

public class LinkwardenTag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class LinkwardenPagedLinks
{
    public List<LinkwardenBookmark> Links { get; set; } = [];
    public int? NextCursor { get; set; }
}

public class LinkwardenApiData<T>
{
    public T Data { get; set; } = default!;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty; 
}

public class LinkwardenApiResponse<T>
{
    public List<T> Response { get; set; } = [];
}
