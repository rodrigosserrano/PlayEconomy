using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Play.Identity.Service.DTOs;

public record CreateUserDTO(
    [Required] string Name,
    [Required] string Nickname,
    [Required, EmailAddress] string Email,
    [Required, PasswordPropertyText, MinLength(8)] string Password,
    double Balance = 0
);