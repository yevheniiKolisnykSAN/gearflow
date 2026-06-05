using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.Reservations;

public class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, Reservation>
{
    private readonly IReservationRepository _repository;

    public GetReservationByIdQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }


    public async Task<Reservation> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdWithDetailsAsync(request.ReservationId);
    }
}