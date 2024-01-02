using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.DTOs;

public record CreateItemDTO(
    [Required] string Name,
    string Description,
    [Range(0, 1000)] decimal Price
);
