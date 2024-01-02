using Play.Inventory.Service.DTOs;

namespace Play.Inventory.Service.Entities;

public static class Extensions
{
    public static InventoryItemDTO AsDto(this InventoryItem inventoryItem)
    {
        return new InventoryItemDTO(
            CatalogItemId: inventoryItem.CatalogItemId,
            Quantity: inventoryItem.Quantity,
            AcquiredDate: inventoryItem.AcquiredDate
        );
    }
}