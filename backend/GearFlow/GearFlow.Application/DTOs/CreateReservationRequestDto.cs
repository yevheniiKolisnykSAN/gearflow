namespace GearFlow.Application.DTOs;

public record CreateReservationRequestDto(int EquipmentId, DateTime StartDate, DateTime EndDate);