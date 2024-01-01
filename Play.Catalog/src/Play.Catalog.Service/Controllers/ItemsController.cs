using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.DTOs;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Tools;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> itemsRepository;
    public ItemsController(IRepository<Item> itemsRepository) => this.itemsRepository = itemsRepository;

    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetAsync()
    {
        var items = (await itemsRepository.GetAll())
                    .Select(item => item.AsDto());
        return items;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
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
    public async Task<ActionResult<ItemDto>> CreateAsync(CreateItemDto createItemDto)
    {
        var item = new Item
        {
            Name = createItemDto.Name,
            Description = createItemDto.Description,
            Price = createItemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await itemsRepository.Create(item);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
    }

    // IActionResult é utilizando apenas quando não existe um objeto de retorno
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateItemDto updateItemDto)
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

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var item = await itemsRepository.Get(id);

        if (item == null)
        {
            return NotFound();
        }

        await itemsRepository.Delete(id);

        return NoContent();
    }
}