using Play.Common;

namespace Play.Identity.Service.Entities;

public class Role : IEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}