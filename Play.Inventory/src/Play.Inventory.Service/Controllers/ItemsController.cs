using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.DTOs;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventoryItem> itemsRepository;
    private readonly IRepository<CatalogItem> catalogRepository;

    public ItemsController(IRepository<InventoryItem> itemsRepository, IRepository<CatalogItem> catalogRepository)
    {
        this.itemsRepository = itemsRepository;
        this.catalogRepository = catalogRepository;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDTO>>> GetAsync()
    {
        var userId = HttpContext.User.Identity!.IsAuthenticated
           ? Guid.Parse(HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value!)
           : Guid.Empty;

        if (userId == Guid.Empty)
        {
            return BadRequest();
        }

        var inventoryItemEntities = await itemsRepository.GetAll(item => item.UserId == userId);
        var itemsIds = inventoryItemEntities.Select(item => item.CatalogItemId);
        var catalogItemsEntities = await catalogRepository.GetAll(item => itemsIds.Contains(item.Id));

        var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
        {
            var catalogItem = catalogItemsEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
            return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
        });

        return Ok(inventoryItemDtos);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(GrantItemsDTO grantItemsDTO)
    {
        var userId = HttpContext.User.Identity!.IsAuthenticated
           ? Guid.Parse(HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value!)
           : Guid.Empty;

        if (userId == Guid.Empty)
        {
            return BadRequest();
        }

        var inventoryItem = await itemsRepository.Get(item =>
                item.UserId == userId && item.CatalogItemId == grantItemsDTO.CatalogItemId
            );

        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem
            {
                CatalogItemId = grantItemsDTO.CatalogItemId,
                UserId = userId,
                Quantity = grantItemsDTO.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };

            await itemsRepository.Create(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grantItemsDTO.Quantity;
            await itemsRepository.Update(inventoryItem);
        }

        return Ok();
    }

}