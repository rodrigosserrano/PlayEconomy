using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Play.Identity.Service.DTOs.Role;

public class CreateUpdateRoleDTO
{
    [Required]
    public required string Name { get; set; }
}