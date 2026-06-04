using GearFlow.Application.DTOs;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.Auth;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await this._userRepository.GetByIdWithRoleAsync(request.UserId);

        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }
        
        return new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.Role.Name);
    }
}