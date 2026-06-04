using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using GearFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GearFlow.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(GearFlowDbContext context) : base(context)
    {
    }


    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdWithRoleAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}