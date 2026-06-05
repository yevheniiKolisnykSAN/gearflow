using GearFlow.Domain.Interfaces;
using MediatR;


namespace GearFlow.Application.Queries.Location;

public class GetAllLocationsQueryHandler : IRequestHandler<GetAllLocationsQuery, IEnumerable<Domain.Entities.Location>>
{
    private readonly IRepository<Domain.Entities.Location> _repository;

    public GetAllLocationsQueryHandler(IRepository<Domain.Entities.Location> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Domain.Entities.Location>> Handle(GetAllLocationsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(l => !l.Archived);
    }
    
}