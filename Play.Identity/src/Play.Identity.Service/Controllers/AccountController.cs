using MassTransit.Initializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Play.Common;
using Play.Identity.Service.DTOs;
using Play.Identity.Service.Entities;

namespace Play.Identity.Service.Controllers;

[ApiController]
[Route("account")]
[Authorize]
public class AccountController(IRepository<User> userRepository) : ControllerBase
{
    private readonly IRepository<User> userRepository = userRepository;

    [HttpGet]
    public async Task<ActionResult<UserDTO>> GetLoggedUser()
    {
        var userId = HttpContext.User.Identity!.IsAuthenticated
            ? Guid.Parse(HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value!)
            : Guid.Empty;

        var user = (await userRepository.Get(userId)).AsDTO();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult<UserDTO>> UpdateAsync(UpdateUserDTO updateUserDTO)
    {
        var userId = HttpContext.User.Identity!.IsAuthenticated
            ? Guid.Parse(HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value!)
            : Guid.Empty;

        var existingUser = await userRepository.Get(i => i.Id == userId);

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
        existingUser.Password = password;

        await userRepository.Update(existingUser);

        return NoContent();
    }
}
