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

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var apiResponse = JsonSerializer.Deserialize<LinkwardenApiResponse<LinkwardenCollection>>(content, options);

        return apiResponse?.Response ?? [];
    }

    public async Task<List<LinkwardenBookmark>> GetBookmarksByCollectionAsync(int collectionId)
    {
        try
        {
            var url = $"{_configuration["LINKWARDEN_API_URL"]}/api/v1/search?collectionId={collectionId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {_configuration["LINKWARDEN_API_KEY"]}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResponse = JsonSerializer.Deserialize<LinkwardenApiData<LinkwardenPagedLinks>>(content, options);

            return apiResponse?.Data?.Links ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching bookmarks for collection {CollectionId}", collectionId);
            throw;
        }
    }

    public async Task<LinkwardenCollection?> GetCollectionWithChildrenAsync(LinkwardenCollection collection)
    {
        // Fetch bookmarks for this collection
        collection.Bookmarks = await GetBookmarksByCollectionAsync(collection.Id);

        // Fetch children collections recursively
        var allCollections = await GetAllCollectionsAsync();
        var childCollections = allCollections.Where(c => c.ParentId == collection.Id).ToList();

        foreach (var child in childCollections)
        {
            collection.Children.Add(await GetCollectionWithChildrenAsync(child) ?? child);
        }

        return collection;
    }
}
