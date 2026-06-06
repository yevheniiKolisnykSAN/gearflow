using FluentAssertions;
using GearFlow.Application.Queries.Auth;
using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Auth;

public class GetCurrentUserQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryHandlerTests()
    {
        _handler = new GetCurrentUserQueryHandler(_userRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingUser_ReturnsUserDto()
    {
        var user = new User
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            RoleId = 2,
            Role = new Role { Id = 2, Name = "User" }
        };
        _userRepoMock.Setup(r => r.GetByIdWithRoleAsync(1)).ReturnsAsync(user);

        var result = await _handler.Handle(new GetCurrentUserQuery(1), default);

        result.Id.Should().Be(1);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john@test.com");
        result.RoleId.Should().Be(2);
        result.RoleName.Should().Be("User");
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        _userRepoMock.Setup(r => r.GetByIdWithRoleAsync(99)).ReturnsAsync((User?)null);

        var act = () => _handler.Handle(new GetCurrentUserQuery(99), default);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User not found");
    }
}
