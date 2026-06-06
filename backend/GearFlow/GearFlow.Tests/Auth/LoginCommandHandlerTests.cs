using FluentAssertions;
using GearFlow.Application.Commands.Auth;
using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();
    private readonly Mock<IJwtService> _jwtMock = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_userRepoMock.Object, _hasherMock.Object, _jwtMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        var user = new User { Email = "user@test.com", PasswordHash = "hashed" };
        _userRepoMock.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);
        _hasherMock.Setup(h => h.VerifyPassword("pass", "hashed")).Returns(true);
        _jwtMock.Setup(j => j.GenerateJwtToken(user)).Returns("jwt-token");

        var result = await _handler.Handle(new LoginCommand("user@test.com", "pass"), default);

        result.Should().Be("jwt-token");
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync("no@test.com")).ReturnsAsync((User?)null);

        var act = () => _handler.Handle(new LoginCommand("no@test.com", "pass"), default);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        var user = new User { Email = "user@test.com", PasswordHash = "hashed" };
        _userRepoMock.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);
        _hasherMock.Setup(h => h.VerifyPassword("wrong", "hashed")).Returns(false);

        var act = () => _handler.Handle(new LoginCommand("user@test.com", "wrong"), default);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");
    }
}
