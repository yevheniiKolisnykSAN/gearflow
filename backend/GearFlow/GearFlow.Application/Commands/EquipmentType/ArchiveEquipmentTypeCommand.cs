using MediatR;

namespace GearFlow.Application.Commands.EquipmentType;

public record ArchiveEquipmentTypeCommand(int Id) : IRequest<bool>;