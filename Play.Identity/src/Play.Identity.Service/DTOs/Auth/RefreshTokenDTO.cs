using System.ComponentModel.DataAnnotations;

namespace Play.Identity.Service.DTOs.Auth;

public record RefreshTokenDTO(
    string AccessToken,
    string RefreshToken
);