using MassTransit.Initializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Play.Common;
using Play.Identity.Service.Auth;
using Play.Identity.Service.DTOs;
using Play.Identity.Service.Entities;
using static Play.Identity.Service.DTOs.ServiceResponse;

namespace Play.Identity.Service.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IRepository<User> userRepository, Token token) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
    {
        var user = new User
        {
            Name = registerDto.Name,
            Nickname = registerDto.Nickname,
            Email = registerDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Balance = 0
        };

        var existingUser = await userRepository.Get(check => check.Email == registerDto.Email);


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

        return Created();
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginDTO login)
    {
        if (login is null)
        {
            return new LoginResponse(null!, "Login container is empty");
        }

        var user = await userRepository.Get(u => u.Email == login.Email);
        if (user is null)
        {
            return new LoginResponse(null!, "Invalid user or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
        {
            return new LoginResponse(null!, "Invalid user or password");
        }

        return new LoginResponse(token.GenerateToken(user.AsDTO()), "Login successful");
    }
}
