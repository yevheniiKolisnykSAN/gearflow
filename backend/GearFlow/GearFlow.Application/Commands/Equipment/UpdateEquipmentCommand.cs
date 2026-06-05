using MediatR;

namespace GearFlow.Application.Commands.Equipment;

public record UpdateEquipmentCommand(int Id, string Name, int LocationId, string Specification, int MaxLoanDays, int StatusId, int TypeId) : IRequest<int>;