using MediatR;

namespace GearFlow.Application.Commands.Reservation;

public record CancelReservationCommand(int id, int UserId, int UserRoleId) : IRequest<bool>;