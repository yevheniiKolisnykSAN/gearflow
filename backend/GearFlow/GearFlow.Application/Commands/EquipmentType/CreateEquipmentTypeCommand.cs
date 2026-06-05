using MediatR;

namespace GearFlow.Application.Commands.EquipmentType;

public record CreateEquipmentTypeCommand(string Name) : IRequest<int>;