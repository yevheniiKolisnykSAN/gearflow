using MediatR;

namespace GearFlow.Application.Commands.Reservation;

public record CompleteReservationCommand(int ReservationId, int UserId, int UserRoleId) : IRequest<bool>;