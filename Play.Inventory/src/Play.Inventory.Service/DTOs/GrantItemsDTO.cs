namespace Play.Inventory.Service.DTOs;

public record GrandItemsDTO(
    Guid UserId,
    Guid CatalogItemId,
    int Quantity
);