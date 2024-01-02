using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.DTOs;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventoryItem> itemsRepository;
    public ItemsController(IRepository<InventoryItem> itemsRepository) => this.itemsRepository = itemsRepository;


    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDTO>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest();
        }

        var items = (await itemsRepository.GetAll(item => item.UserId == userId))
                    .Select(items => items.AsDto());

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(GrandItemsDTO grandItemsDTO)
    {
        var inventoryItem = await itemsRepository.Get(item =>
                item.UserId == grandItemsDTO.UserId && item.CatalogItemId == grandItemsDTO.CatalogItemId
            );

        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem
            {
                CatalogItemId = grandItemsDTO.CatalogItemId,
                UserId = grandItemsDTO.UserId,
                Quantity = grandItemsDTO.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };

            await itemsRepository.Create(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grandItemsDTO.Quantity;
            await itemsRepository.Update(inventoryItem);
        }

        return Ok();
    }

}