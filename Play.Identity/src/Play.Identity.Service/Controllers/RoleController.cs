using MassTransit.Initializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Identity.Service.DTOs.Role;
using Play.Identity.Service.Entities;

namespace Play.Identity.Service.Controllers;

[ApiController]
[Route("roles")]
[Authorize(Roles = "admin")]
public class RoleController : ControllerBase
{
    private readonly IRepository<Role> roleRepository;

    public RoleController(IRepository<Role> roleRepository)
    {
        this.roleRepository = roleRepository;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDTO>>> GetAll()
    {
        var roles = (await roleRepository.GetAll())
                .Select(item => item.AsDTO());

        if (roles.Count() <= 0)
        {
            return NotFound();
        }

        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<RoleDTO>>> Get(Guid id)
    {
        var role = (await roleRepository.Get(id)).AsDTO();

        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult> CreateRole(CreateUpdateRoleDTO createRoleDTO)
    {
        var role = new Role
        {
            Name = createRoleDTO.Name
        };

        await roleRepository.Create(role);

        return CreatedAtAction(nameof(Get), new { id = role.Id }, role);
    }
}