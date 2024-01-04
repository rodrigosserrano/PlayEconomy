using Play.Inventory.Service.DTOs;

namespace Play.Inventory.Service.Entities;

public static class Extensions
{
    public static InventoryItemDTO AsDto(this InventoryItem inventoryItem, string name, string description)
    {
        return new InventoryItemDTO(
            CatalogItemId: inventoryItem.CatalogItemId,
            Name: name,
            Description: description,
            Quantity: inventoryItem.Quantity,
            AcquiredDate: inventoryItem.AcquiredDate
        );
    }
}