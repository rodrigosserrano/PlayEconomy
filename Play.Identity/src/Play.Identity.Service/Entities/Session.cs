using Play.Common;

namespace Play.Identity.Service.Entities;

public class Session : IEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string AccessToken { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public required string RefreshToken { get; set; }
    public required bool IsValid { get; set; }
}
