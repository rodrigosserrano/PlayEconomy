using Play.Common;

namespace Play.Identity.Service.Entities;

public class User : IEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Nickname { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public double Balance { get; set; }
}
