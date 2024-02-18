using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Common.Auth;
using Play.Common.Auth.DTOs;
using Play.Identity.Service.DTOs.User;
using Play.Identity.Service.DTOs.Auth;
using Play.Identity.Service.Entities;

namespace Play.Identity.Service.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IRepository<User> userRepository, Auth auth, IRepository<Session> sessionRepository) : ControllerBase
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
            Balance = 0,
            Role = "user"
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
    public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO login)
    {
        if (login is null)
        {
            return BadRequest("Login container is empty");
        }

        var user = await userRepository.Get(u => u.Email == login.Email);
        if (user is null)
        {
            return BadRequest("Invalid user or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
        {
            return BadRequest("Invalid user or password");
        }

        var tokenInfo = auth.GenAccessToken(user.AsIdentity());

        await sessionRepository.Create(new Session
        {
            UserId = user.Id,
            AccessToken = tokenInfo.AccessToken,
            ExpiresAt = DateTime.UtcNow.AddSeconds(tokenInfo.ExpiresIn),
            IsValid = true,
            RefreshToken = tokenInfo.RefreshToken
        });

        return Ok(tokenInfo);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDTO>> RefreshToken(RefreshTokenDTO refreshTokenDto)
    {
        var session = await sessionRepository.Get(s => s.AccessToken == refreshTokenDto.AccessToken);

        if (session.RefreshToken != refreshTokenDto.RefreshToken || session.ExpiresAt < DateTime.UtcNow)
        {
            return Unauthorized();
        }

        var user = await userRepository.Get(session.UserId);

        session.IsValid = false;

        await sessionRepository.Update(session);

        var tokenInfo = auth.GenAccessToken(user.AsIdentity());

        await sessionRepository.Create(new Session
        {
            UserId = user.Id,
            AccessToken = tokenInfo.AccessToken,
            ExpiresAt = DateTime.UtcNow.AddSeconds(tokenInfo.ExpiresIn),
            IsValid = true,
            RefreshToken = tokenInfo.RefreshToken
        });

        return Ok(tokenInfo);
    }

}
