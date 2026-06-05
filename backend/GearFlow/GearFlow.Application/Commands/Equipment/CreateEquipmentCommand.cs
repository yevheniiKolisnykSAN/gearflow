using MediatR;

namespace GearFlow.Application.Commands.Equipment;

public record CreateEquipmentCommand(string SerialNumber, string Name, string Specification, int MaxLoanDays, int StatusId, int TypeId, int LocationId) : IRequest<int>;