using FluentAssertions;
using GearFlow.Application.Commands.Location;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Location;

public class ArchiveLocationCommandHandlerTests
{
    private readonly Mock<IRepository<GearFlow.Domain.Entities.Location>> _repoMock = new();
    private readonly ArchiveLocationCommandHandler _handler;

    public ArchiveLocationCommandHandlerTests()
    {
        _handler = new ArchiveLocationCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ActiveLocation_ArchivesAndReturnsTrue()
    {
        var location = new GearFlow.Domain.Entities.Location { Id = 1, Archived = false };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(location);
        _repoMock.Setup(r => r.UpdateAsync(location)).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new ArchiveLocationCommand(1), default);

        result.Should().BeTrue();
        location.Archived.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((GearFlow.Domain.Entities.Location?)null);

        var act = () => _handler.Handle(new ArchiveLocationCommand(99), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Location not found");
    }

    [Fact]
    public async Task Handle_AlreadyArchived_ThrowsInvalidOperationException()
    {
        var location = new GearFlow.Domain.Entities.Location { Id = 1, Archived = true };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(location);

        var act = () => _handler.Handle(new ArchiveLocationCommand(1), default);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Location already deleted");
    }
}
