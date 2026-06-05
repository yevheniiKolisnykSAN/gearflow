using System.Linq.Expressions;
using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using GearFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GearFlow.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly GearFlowDbContext _context;

    public Repository(GearFlowDbContext GearFlowDbContext)
    {
        _context = GearFlowDbContext;
    }


    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
    {
        var query =  _context.Set<T>().AsQueryable();
        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;
    
        _context.Set<T>().Remove(entity);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}