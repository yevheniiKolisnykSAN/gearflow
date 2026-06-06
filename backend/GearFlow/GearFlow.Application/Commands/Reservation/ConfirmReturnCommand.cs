using MediatR;

namespace GearFlow.Application.Commands.Reservation;

public record ConfirmReturnCommand(int ReservationId, string? DefectComment = null) : IRequest<bool>;