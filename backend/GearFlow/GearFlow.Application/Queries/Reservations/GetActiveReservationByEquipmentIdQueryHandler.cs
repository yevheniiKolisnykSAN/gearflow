using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public class GetActiveReservationByEquipmentIdQueryHandler : IRequestHandler<GetActiveReservationByEquipmentIdQuery, Reservation?>
{
    private readonly IReservationRepository _repository;

    public GetActiveReservationByEquipmentIdQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }


    public async Task<Reservation?> Handle(GetActiveReservationByEquipmentIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetActiveByEquipmentIdAsync(request.EquipmentId);
    }
}