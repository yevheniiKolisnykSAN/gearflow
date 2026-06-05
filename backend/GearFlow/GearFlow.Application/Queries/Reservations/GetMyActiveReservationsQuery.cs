using GearFlow.Domain.Entities;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public record GetMyActiveReservationsQuery(int UserId) : IRequest<IEnumerable<Reservation>>;