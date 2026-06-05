using MediatR;

namespace GearFlow.Application.Commands.EquipmentType;

public record UpdateEquipmentTypeCommand(int Id, string Name) : IRequest<int>;