namespace Play.Identity.Service.DTOs;

public record UserDTO(
    Guid Id,
    string Name,
    string Nickname,
    string Email,
    double Balance
);