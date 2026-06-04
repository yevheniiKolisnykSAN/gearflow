using GearFlow.Domain.Entities;

namespace GearFlow.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    public Task<User?> GetByEmailAsync(string email);

    public Task<User?> GetByIdWithRoleAsync(int id);
}