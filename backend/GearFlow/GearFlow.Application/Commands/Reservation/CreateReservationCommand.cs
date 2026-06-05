using MediatR;

namespace GearFlow.Application.Commands.Reservation;

public record CreateReservationCommand(int EquipmentId, DateTime StartDate, DateTime EndDate, int UserId) : IRequest<int>;