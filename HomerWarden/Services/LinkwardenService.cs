using HomerWarden.Models;
using System.Text.Json;

namespace HomerWarden.Services;

public sealed class LinkwardenService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LinkwardenService> _logger;
    private readonly IConfiguration _configuration;

    public LinkwardenService(HttpClient httpClient, ILogger<LinkwardenService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<List<LinkwardenCollection>> GetHomerCollectionsAsync()
    {
        try
        {
            var collections = await GetAllCollectionsAsync();
            return collections.Where(c => c.Name.StartsWith("homer:")).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Homer collections from Linkwarden");
            throw;
        }
    }

    private async Task<List<LinkwardenCollection>> GetAllCollectionsAsync()
    {
        var url = $"{_configuration["LINKWARDEN_API_URL"]}/api/v1/collections";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Authorization", $"Bearer {_configuration["LINKWARDEN_API_KEY"]}");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStreamAsync();
        var apiResponse = JsonSerializer.Deserialize(content, AppJsonSerializerContext.Default.LinkwardenApiResponseLinkwardenCollection);

        return apiResponse?.Response ?? [];
    }

    private async Task<List<LinkwardenBookmark>> GetBookmarksByCollectionAsync(int collectionId)
    {
        try
        {
            var url = $"{_configuration["LINKWARDEN_API_URL"]}/api/v1/search?collectionId={collectionId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {_configuration["LINKWARDEN_API_KEY"]}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStreamAsync();
            var apiResponse = JsonSerializer.Deserialize(content, AppJsonSerializerContext.Default.LinkwardenApiDataLinkwardenPagedLinks);

            return apiResponse?.Data?.Links ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching bookmarks for collection {CollectionId}", collectionId);
            throw;
        }
    }

    public async Task<LinkwardenCollection?> GetCollectionWithChildrenAsync(LinkwardenCollection collection, List<LinkwardenCollection>? allCollections = null)
    {
        // Fetch bookmarks for this collection
        collection.Bookmarks = await GetBookmarksByCollectionAsync(collection.Id);

        // Fetch children collections recursively
        allCollections ??= await GetAllCollectionsAsync();
        var childCollections = allCollections.Where(c => c.ParentId == collection.Id).ToList();

        foreach (var child in childCollections)
        {
            var processedChild = await GetCollectionWithChildrenAsync(child, allCollections);
            collection.Children.Add(processedChild ?? child);
        }

        return collection;
    }
}
