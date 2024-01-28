using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Play.Identity.Service.DTOs;
using Play.Identity.Service.Settings;

namespace Play.Identity.Service.Auth;

public class Token(IConfiguration config)
{

    public string GenerateToken(UserDTO user)
    {
        var authSettings = config.GetSection(nameof(AuthSettings)).Get<AuthSettings>()!;

        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var token = new JwtSecurityToken(
            issuer: authSettings.Issuer,
            audience: authSettings.Audience,
            claims: userClaims,
            expires: DateTime.Now.Add(TimeSpan.FromSeconds(authSettings.ExpirationTimeSeconds)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}