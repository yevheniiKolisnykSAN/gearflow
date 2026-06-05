using GearFlow.Domain.Entities;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public record GetReservationByIdQuery(int ReservationId) : IRequest<Reservation>;