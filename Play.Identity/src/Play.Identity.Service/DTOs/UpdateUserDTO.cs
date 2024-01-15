using System.ComponentModel.DataAnnotations;

namespace Play.Identity.Service.DTOs;

public record UpdateUserDTO(
    [Required] string Name,
    [Required] string Nickname,
    [Required, EmailAddress] string Email,
    [Required] string Password
);