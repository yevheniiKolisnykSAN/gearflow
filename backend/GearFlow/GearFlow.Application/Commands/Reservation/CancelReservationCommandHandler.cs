using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Reservation;

public class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, bool>
{
    private readonly IReservationRepository _repository;

    public CancelReservationCommandHandler(IReservationRepository repository)
    {
        _repository = repository;
    }


    public async Task<bool> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _repository.GetByIdAsync(request.id);
        
        if (reservation == null)
        {
            throw new KeyNotFoundException("Reservation not found");
        }

        if (reservation.Status != ReservationStatus.Active)
        {
            throw new InvalidOperationException("Only active reservations can be canceled");
        }
                
        if (request.UserRoleId == 2 && reservation.UserId != request.UserId)
        {
            throw new InvalidOperationException("Only user can complete reservation");
        }

        
        if (reservation.StartDate > DateTime.UtcNow)
        {
            reservation.Status = ReservationStatus.Cancelled;
            await _repository.UpdateAsync(reservation);
            return true;
        }
        
        

        throw new InvalidOperationException("Cannot cancel reservation that has already started");
    }
}