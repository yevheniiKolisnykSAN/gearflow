using GearFlow.Domain.Entities;

namespace GearFlow.Application.DTOs;

public record GetAdminReservationsResponseDto(IEnumerable<Reservation> Active, IEnumerable<Reservation> History, IEnumerable<Reservation> Pending);