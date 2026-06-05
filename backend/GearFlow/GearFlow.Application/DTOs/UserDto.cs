namespace GearFlow.Application.DTOs;

public record UserDto(int Id, string FirstName, string LastName, string Email, int RoleId, string RoleName);