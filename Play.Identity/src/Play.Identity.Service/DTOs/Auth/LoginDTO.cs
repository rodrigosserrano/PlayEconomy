using System.ComponentModel.DataAnnotations;

namespace Play.Identity.Service.DTOs.Auth;

public class LoginDTO
{
    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required, DataType(DataType.Password), MinLength(8)]
    public required string Password { get; set; }
}