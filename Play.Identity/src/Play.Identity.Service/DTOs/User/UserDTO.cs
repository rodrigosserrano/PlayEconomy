namespace Play.Identity.Service.DTOs.User;

public record UserDTO(
    Guid Id,
    string Name,
    string Nickname,
    string Email,
    string Password,
    double Balance,
    string Role
);