using GearFlow.Domain.Entities;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public record GetActiveReservationByEquipmentIdQuery(int EquipmentId) : IRequest<Reservation?>;