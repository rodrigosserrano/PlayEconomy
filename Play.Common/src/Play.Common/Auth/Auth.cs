using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Play.Common.Auth.DTOs;
using Play.Common.Settings;

namespace Play.Common.Auth;

public class Auth
{

    public AuthResponseDTO GenAccessToken(IdentityDTO user)
    {
        var authSettings = new AuthSettings
        {
            Key = "0pqVdXBxfBpkTZuVoPh1bjoIiOyVx1Uj",
            Audience = [
                "https://localhost:7103",
                "https://localhost:7194"
            ],
            ExpirationTimeSeconds = 3600,
            Issuer = "https://localhost:7103"
        };

        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role),
        };


        foreach (var audience in authSettings.Audience)
        {
            userClaims.Add(new Claim("aud", audience));
        }

        var token = new JwtSecurityToken(
            issuer: authSettings.Issuer,
            claims: userClaims,
            expires: DateTime.Now.Add(TimeSpan.FromSeconds(authSettings.ExpirationTimeSeconds)),
            signingCredentials: credentials
        );

        return new AuthResponseDTO(
            AccessToken: new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresIn: authSettings.ExpirationTimeSeconds,
            RefreshToken: GenRefreshToken()
        );
    }

    public string GenRefreshToken()
    {
        var randomNumber = new byte[64];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}