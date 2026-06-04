using GearFlow.Domain.Entities;

namespace GearFlow.Domain.Interfaces;

public interface IJwtService
{
    public string GenerateJwtToken(User user);
}