namespace Play.Inventory.Service.DTOs;

public record InventoryItemDTO(
    Guid CatalogItemId,
    int Quantity,
    DateTimeOffset AcquiredDate
);