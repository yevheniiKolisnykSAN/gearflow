using FluentAssertions;
using GearFlow.Application.Commands.EquipmentType;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.EquipmentType;

public class ArchiveEquipmentTypeCommandHandlerTests
{
    private readonly Mock<IRepository<GearFlow.Domain.Entities.EquipmentType>> _repoMock = new();
    private readonly ArchiveEquipmentTypeCommandHandle _handler;

    public ArchiveEquipmentTypeCommandHandlerTests()
    {
        _handler = new ArchiveEquipmentTypeCommandHandle(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ActiveType_ArchivesAndReturnsTrue()
    {
        var type = new GearFlow.Domain.Entities.EquipmentType { Id = 1, Archived = false };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(type);
        _repoMock.Setup(r => r.UpdateAsync(type)).Returns(Task.CompletedTask);

        var result = await _handler.Handle(new ArchiveEquipmentTypeCommand(1), default);

        result.Should().BeTrue();
        type.Archived.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((GearFlow.Domain.Entities.EquipmentType?)null);

        var act = () => _handler.Handle(new ArchiveEquipmentTypeCommand(99), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Equipment type not found");
    }

    [Fact]
    public async Task Handle_AlreadyArchived_ThrowsInvalidOperationException()
    {
        var type = new GearFlow.Domain.Entities.EquipmentType { Id = 1, Archived = true };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(type);

        var act = () => _handler.Handle(new ArchiveEquipmentTypeCommand(1), default);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Equipment type already deleted");
    }
}
