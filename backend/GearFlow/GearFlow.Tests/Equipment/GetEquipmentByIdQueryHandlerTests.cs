using FluentAssertions;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Equipment;

public class GetEquipmentByIdQueryHandlerTests
{
    private readonly Mock<IEquipmentRepository> _repoMock = new();
    private readonly GetEquipmentByIdQueryHandler _handler;

    public GetEquipmentByIdQueryHandlerTests()
    {
        _handler = new GetEquipmentByIdQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsEquipment()
    {
        var equipment = new GearFlow.Domain.Entities.Equipment { Id = 1, Name = "Drill" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(equipment);

        var result = await _handler.Handle(new GetEquipmentByIdQuery(1), default);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Drill");
    }

    [Fact]
    public async Task Handle_NonExistingId_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((GearFlow.Domain.Entities.Equipment?)null);

        var result = await _handler.Handle(new GetEquipmentByIdQuery(99), default);

        result.Should().BeNull();
    }
}
