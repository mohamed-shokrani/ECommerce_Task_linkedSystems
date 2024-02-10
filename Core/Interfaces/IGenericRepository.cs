using Core.Models;
using System.Linq.Expressions;

namespace Core.Interfaces;
public interface IGenericRepository<T> where T : Entity
{
    Task<T> GetById(int id);
    Task<bool> Update( T updatedEntity);
    Task<int> DeleteAsync(Expression<Func<T, bool>> expression);
    Task AddAsync(T entity);
    Task GetAllAsync();
    IQueryable<T> GetAllQueryable();
}
