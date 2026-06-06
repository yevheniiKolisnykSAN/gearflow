using FluentAssertions;
using GearFlow.Application.Commands.Equipment;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Equipment;

public class ArchiveEquipmentCommandHandlerTests
{
    private readonly Mock<IRepository<GearFlow.Domain.Entities.Equipment>> _repoMock = new();
    private readonly ArchiveEquipmentCommandHandler _handler;

    public ArchiveEquipmentCommandHandlerTests()
    {
        _handler = new ArchiveEquipmentCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingActiveEquipment_ArchivesAndReturnsTrue()
    {
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 1, Archived = false };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(equipment);
        _repoMock.Setup(r => r.UpdateAsync(equipment)).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new ArchiveEquipmentCommand(1), default);

        result.Should().BeTrue();
        equipment.Archived.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((GearFlow.Domain.Entities.Equipment?)null);

        var act = () => _handler.Handle(new ArchiveEquipmentCommand(99), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Equipment not found");
    }

    [Fact]
    public async Task Handle_AlreadyArchived_ThrowsInvalidOperationException()
    {
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 1, Archived = true };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(equipment);

        var act = () => _handler.Handle(new ArchiveEquipmentCommand(1), default);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Equipment already deleted");
    }
}
