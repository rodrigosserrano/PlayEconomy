using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Play.Identity.Service.DTOs.Auth;

public class RegisterDTO
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Nickname { get; set; }

    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required, DataType(DataType.Password), MinLength(8)]
    public required string Password { get; set; }

    [Required, DataType(DataType.Password), MinLength(8), Compare("Password")]
    public required string ConfirmPassword { get; set; }
}