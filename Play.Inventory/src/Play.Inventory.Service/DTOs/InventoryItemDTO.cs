namespace Play.Inventory.Service.DTOs;

public record InventoryItemDTO(
    Guid CatalogItemId,
    string Name,
    string Description,
    int Quantity,
    DateTimeOffset AcquiredDate
);