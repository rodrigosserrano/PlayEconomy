using Play.Common;

namespace Play.Identity.Service.Entities;

public class User : IEntity
{
    public Guid Id { get; set; }
    public String Name { get; set; }
    public String Nickname { get; set; }
    public String Email { get; set; }
    public String Password { get; set; }
}
