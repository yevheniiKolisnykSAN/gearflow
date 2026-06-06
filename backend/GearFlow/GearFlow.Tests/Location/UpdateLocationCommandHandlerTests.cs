using FluentAssertions;
using GearFlow.Application.Commands.Location;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Location;

public class UpdateLocationCommandHandlerTests
{
    private readonly Mock<IRepository<GearFlow.Domain.Entities.Location>> _repoMock = new();
    private readonly UpdateLocationCommandHandle _handler;

    public UpdateLocationCommandHandlerTests()
    {
        _handler = new UpdateLocationCommandHandle(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingLocation_UpdatesAndReturnsId()
    {
        var location = new GearFlow.Domain.Entities.Location { Id = 3, Name = "Old Name" };
        _repoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(location);
        _repoMock.Setup(r => r.UpdateAsync(location)).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new UpdateLocationCommand(3, "New Name"), default);

        result.Should().Be(3);
        location.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task Handle_NotFoundLocation_ReturnsZero()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((GearFlow.Domain.Entities.Location?)null);

        var result = await _handler.Handle(new UpdateLocationCommand(99, "X"), default);

        result.Should().Be(0);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<GearFlow.Domain.Entities.Location>()), Times.Never);
    }
}
