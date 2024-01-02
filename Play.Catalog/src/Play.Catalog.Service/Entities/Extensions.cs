using Play.Catalog.Service.DTOs;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Tools;

public static class Extensions
{
    public static ItemDTO AsDto(this Item item)
    {
        return new ItemDTO(
            Id: item.Id,
            Name: item.Name,
            Description: item.Description,
            Price: item.Price,
            CreatedDate: item.CreatedDate
        );
    }
}
