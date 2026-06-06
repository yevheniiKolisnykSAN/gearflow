using FluentAssertions;
using GearFlow.Application.Commands.Auth;
using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(_userRepoMock.Object, _hasherMock.Object);
    }

    [Fact]
    public async Task Handle_NewEmail_CreatesUserAndReturnsTrue()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync((User?)null);
        _hasherMock.Setup(h => h.HashPassword("pass")).Returns("hashed");
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new RegisterCommand("John", "Doe", "test@test.com", "pass"), default);

        result.Should().BeTrue();
        _userRepoMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Email == "test@test.com" &&
            u.FirstName == "John" &&
            u.LastName == "Doe" &&
            u.PasswordHash == "hashed" &&
            u.RoleId == 2)), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingEmail_ThrowsInvalidOperationException()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync("exists@test.com"))
            .ReturnsAsync(new User { Email = "exists@test.com" });

        var act = () => _handler.Handle(new RegisterCommand("A", "B", "exists@test.com", "pass"), default);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User with this email already exists");
    }
}
