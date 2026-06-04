using MediatR;

namespace GearFlow.Application.Commands.Equipment;

public record CreateEquipmentCommand(string SerialNumber, string Location, string Specification, int MaxLoanDays, int StatusId, int TypeId) : IRequest<int>;