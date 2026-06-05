using MediatR;

namespace GearFlow.Application.Commands.Equipment;

public record ArchiveEquipmentCommand(int Id) : IRequest<bool>;