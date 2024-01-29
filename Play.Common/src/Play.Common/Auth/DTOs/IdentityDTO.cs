namespace Play.Common.Auth.DTOs;

public record IdentityDTO(
    Guid Id,
    string Name,
    string Email
);