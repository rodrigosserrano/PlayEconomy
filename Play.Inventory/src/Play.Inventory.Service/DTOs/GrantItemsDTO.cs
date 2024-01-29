namespace Play.Inventory.Service.DTOs;

public record GrantItemsDTO(
    Guid CatalogItemId,
    int Quantity
);