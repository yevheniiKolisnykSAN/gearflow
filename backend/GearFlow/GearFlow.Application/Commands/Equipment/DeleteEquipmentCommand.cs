using MediatR;

namespace GearFlow.Application.Commands.Equipment;

public record DeleteEquipmentCommand(int Id) : IRequest<bool>;