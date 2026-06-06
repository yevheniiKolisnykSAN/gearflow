using GearFlow.Application.DTOs;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public record GetAdminReservationsQuery() : IRequest<GetAdminReservationsResponseDto>;