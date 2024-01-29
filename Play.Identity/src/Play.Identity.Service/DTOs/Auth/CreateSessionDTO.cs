namespace Play.Identity.Service.DTOs.Auth;

public record CreateSessionDTO(
    Guid UserId,
    string AccessToken,
    DateTime ExpiresAt,
    string RefreshToken,
    bool IsValid
);