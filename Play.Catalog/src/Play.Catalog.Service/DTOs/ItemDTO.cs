using System;

namespace Play.Catalog.Service.DTOs;

public record ItemDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    DateTimeOffset CreatedDate
);

