namespace Play.Identity.Service.DTOs;

public record UserDTO(
    string Name,
    string Nickname,
    string Email
);