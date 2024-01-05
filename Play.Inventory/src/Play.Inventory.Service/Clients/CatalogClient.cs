using Play.Inventory.Service.DTOs;

namespace Play.Inventory.Service.Clients;

public class CatalogClient
{
    private readonly HttpClient httpClient;
    public CatalogClient(HttpClient httpClient) => this.httpClient = httpClient;

    public async Task<IReadOnlyCollection<CatalogItemDTO>> GetCatalogItemsAsync()
    {
        var items = await httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDTO>>("items");
        return items;
    }
}