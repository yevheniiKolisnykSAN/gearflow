using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var verifiedPassword = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

        if (verifiedPassword == false) 
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        return _jwtService.GenerateJwtToken(user);
    }
}