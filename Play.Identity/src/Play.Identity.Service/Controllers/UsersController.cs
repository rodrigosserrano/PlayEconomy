using MassTransit.Initializers;
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
    public async Task<ActionResult<UserDTO>> GetByIdAsync(Guid id)
    {
        return Ok((await userRepository.Get(id)).AsDTO());
    }

    [HttpPost]
    public async Task<ActionResult<UserDTO>> CreateAsync(CreateUserDTO createUserDTO)
    {
        var user = new User
        {
            Name = createUserDTO.Name,
            Nickname = createUserDTO.Nickname,
            Email = createUserDTO.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(createUserDTO.Password)
        };

        var existingUser = await userRepository.Get(check => check.Email == createUserDTO.Email);

        // if (existingUser != null)
        // {
        //     return BadRequest();
        // }

        try
        {
            await userRepository.Create(user);
        }
        catch (MongoWriteException e)
        {
            if (e.WriteError.Code == 11000)
            {
                string[] err = ["Email already in use."];
                return BadRequest(new
                {
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    title = "One or more validation errors occurred.",
                    status = 400,
                    errors = new
                    {
                        Email = err
                    },
                    traceId = HttpContext.TraceIdentifier,
                });
            }
        }

        return CreatedAtAction(nameof(GetByIdAsync), new { id = user.Id }, user.AsDTO());
    }
}