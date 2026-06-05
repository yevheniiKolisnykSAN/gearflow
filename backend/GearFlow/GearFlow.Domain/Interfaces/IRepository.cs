using System.Linq.Expressions;
using GearFlow.Domain.Entities;

namespace GearFlow.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}