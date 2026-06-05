using GearFlow.Application.DTOs;
using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Auth;

public class GetOrCreateMicrosoftUserCommandHandler : IRequestHandler<GetOrCreateMicrosoftUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetOrCreateMicrosoftUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetOrCreateMicrosoftUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            var newUser = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = 2
            };
            await _userRepository.AddAsync(newUser);
            var createdUser = await _userRepository.GetByIdWithRoleAsync(newUser.Id);
            return new UserDto(createdUser!.Id, createdUser.FirstName, createdUser.LastName, createdUser.Email, createdUser.RoleId, createdUser.Role.Name);
        }

        var userWithRole = await _userRepository.GetByIdWithRoleAsync(user.Id);
        return new UserDto(userWithRole!.Id, userWithRole.FirstName, userWithRole.LastName, userWithRole.Email, userWithRole.RoleId, userWithRole.Role.Name);
    }
}