using Microsoft.AspNetCore.Identity;
namespace Play.Common;

public interface IEntityIdentity<T> : IUserStore<T> where T : class
{
    Guid Id { get; set; }
}
