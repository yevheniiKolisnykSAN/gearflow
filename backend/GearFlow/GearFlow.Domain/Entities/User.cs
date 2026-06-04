using System.Collections;

namespace GearFlow.Domain.Entities;

public class User : BaseEntity
{
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public string? PasswordHash { get; set; }

    public Role Role { get; set; }
}