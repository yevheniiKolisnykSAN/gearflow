using FluentAssertions;
using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using Moq;

namespace GearFlow.Tests.Equipment;

public class GetAllEquipmentQueryHandlerTests
{
    private readonly Mock<IEquipmentRepository> _repoMock = new();
    private readonly GetAllEquipmentQueryHandler _handler;

    public GetAllEquipmentQueryHandlerTests()
    {
        _handler = new GetAllEquipmentQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllEquipment()
    {
        var equipment = new List<GearFlow.Domain.Entities.Equipment>
        {
            new() { Id = 1, Name = "Drill" },
            new() { Id = 2, Name = "Saw" }
        };
        _repoMock.Setup(r => r.GetEquipmentList(null, null, null, null, null, null, null, null))
            .ReturnsAsync(equipment);

        var result = await _handler.Handle(new GetAllEquipmentQuery(), default);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithSearchFilter_PassesFilterToRepository()
    {
        _repoMock.Setup(r => r.GetEquipmentList("drill", null, null, null, null, null, null, null))
            .ReturnsAsync(new List<GearFlow.Domain.Entities.Equipment> { new() { Id = 1, Name = "Drill" } });

        var result = await _handler.Handle(new GetAllEquipmentQuery(Search: "drill"), default);

        result.Should().HaveCount(1);
    }
}
