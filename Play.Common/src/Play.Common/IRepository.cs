using System.Linq.Expressions;

namespace Play.Common;

public interface IRepository<T> where T : IEntity
{
    Task Create(T entity);
    Task<IReadOnlyCollection<T>> GetAll();
    Task<IReadOnlyCollection<T>> GetAll(Expression<Func<T, bool>> filter);
    Task<T> Get(Guid id);
    Task<T> Get(Expression<Func<T, bool>> filter);
    Task Delete(Guid id);
    Task Update(T entity);
}
