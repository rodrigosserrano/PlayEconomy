namespace Play.Common.Auth.DTOs;

public record AuthResponseDTO(
    string AccessToken,
    int ExpiresIn,
    string RefreshToken
);