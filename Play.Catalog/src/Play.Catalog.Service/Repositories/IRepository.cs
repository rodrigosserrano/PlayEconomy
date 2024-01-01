namespace Play.Catalog.Service.Repositories;

public interface IRepository<TEntity>
{
    Task Create(TEntity entity);
    Task<IReadOnlyCollection<TEntity>> GetAll();
    Task<TEntity> Get(Guid id);
    Task Delete(Guid id);
    Task Update(TEntity entity);
}
