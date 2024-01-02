using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories;

public interface IRepository<T> where T : IEntity
{
    Task Create(T entity);
    Task<IReadOnlyCollection<T>> GetAll();
    Task<T> Get(Guid id);
    Task Delete(Guid id);
    Task Update(T entity);
}
