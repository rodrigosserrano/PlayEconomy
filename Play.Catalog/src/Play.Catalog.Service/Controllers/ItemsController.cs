using System.ComponentModel;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.DTOs;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Tools;
using Play.Common;
using Play.Catalog.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> itemsRepository;
    private readonly IPublishEndpoint publishEndpoint;

    public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
    {
        this.itemsRepository = itemsRepository;
        this.publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDTO>>> GetAsync()
    {
        var items = (await itemsRepository.GetAll())
                    .Select(item => item.AsDto());

        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDTO>> GetByIdAsync(Guid id)
    {
        var item = await itemsRepository.Get(id);

        if (item == null)
        {
            return NotFound();
        }

        return item.AsDto();
    }

    // ActionResult é utilizado quando terá um objeto de retorno específico
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ItemDTO>> CreateAsync(CreateItemDTO createItemDto)
    {
        var item = new Item
        {
            Name = createItemDto.Name,
            Description = createItemDto.Description,
            Price = createItemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await itemsRepository.Create(item);

        await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
    }

    // IActionResult é utilizando apenas quando não existe um objeto de retorno
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateItemDTO updateItemDto)
    {
        var existingItem = await itemsRepository.Get(id);

        if (existingItem == null)
        {
            return NotFound();
        }

        existingItem.Name = updateItemDto.Name;
        existingItem.Description = updateItemDto.Description;
        existingItem.Price = updateItemDto.Price;

        await itemsRepository.Update(existingItem);

        await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var item = await itemsRepository.Get(id);

        if (item == null)
        {
            return NotFound();
        }

        await itemsRepository.Delete(id);

        await publishEndpoint.Publish(new CatalogItemDeleted(id));

        return NoContent();
    }
}