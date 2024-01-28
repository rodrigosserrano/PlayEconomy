using MassTransit.Initializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Play.Common;
using Play.Identity.Service.DTOs;
using Play.Identity.Service.Entities;

namespace Play.Identity.Service.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IRepository<User> userRepository;

    public UsersController(IRepository<User> userRepository) => this.userRepository = userRepository;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAsync()
    {
        var users = (await userRepository.GetAll())
                    .Select(user => user.AsDTO());


        if (users.Count() == 0)
        {
            return NotFound();
        }

        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UserDTO>> GetByIdAsync(Guid id)
    {
        var user = (await userRepository.Get(id)).AsDTO();

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<UserDTO>> CreateAsync(CreateUserDTO createUserDTO)
    {
        var user = new User
        {
            Name = createUserDTO.Name,
            Nickname = createUserDTO.Nickname,
            Email = createUserDTO.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(createUserDTO.Password),
            Balance = createUserDTO.Balance
        };

        var existingUser = await userRepository.Get(check => check.Email == createUserDTO.Email);


        if (existingUser != null)
        {
            return BadRequest(new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                title = "One or more validation errors occurred.",
                status = 400,
                errors = "User already exists.",
                traceId = HttpContext.TraceIdentifier,
            });
        }

        await userRepository.Create(user);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = user.Id }, user.AsDTO());
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<UserDTO>> UpdateAsync(UpdateUserDTO updateUserDTO, Guid id)
    {
        var existingUser = await userRepository.Get(i => i.Id == id);

        if (existingUser == null)
        {
            return NotFound();
        }

        if (!BCrypt.Net.BCrypt.Verify(updateUserDTO.OldPassword, existingUser.Password))
        {
            return BadRequest(new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                title = "One or more validation errors occurred.",
                status = 400,
                errors = "Incorrect password.",
                traceId = HttpContext.TraceIdentifier,
            });
        }

        var password = (updateUserDTO.OldPassword == updateUserDTO.NewPassword)
            ? existingUser.Password
            : BCrypt.Net.BCrypt.HashPassword(updateUserDTO.NewPassword);

        existingUser.Name = updateUserDTO.Name;
        existingUser.Nickname = updateUserDTO.Nickname;
        existingUser.Email = updateUserDTO.Email;
        existingUser.Balance = updateUserDTO.Balance;
        existingUser.Password = password;

        await userRepository.Update(existingUser);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        var existingUser = await userRepository.Get(id);

        if (existingUser == null)
        {
            return NotFound();
        }

        await userRepository.Delete(id);

        return NoContent();
    }
}
