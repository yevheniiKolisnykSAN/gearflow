using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.EquipmentStatus;

public class GetAllEquipmentStatusesQueryHandler : IRequestHandler<GetAllEquipmentStatusesQuery, IEnumerable<Domain.Entities.EquipmentStatus>>
{
    private readonly IRepository<Domain.Entities.EquipmentStatus> _repository;

    public GetAllEquipmentStatusesQueryHandler(IRepository<Domain.Entities.EquipmentStatus> repository)
    {
        _repository = repository;
    }


    public async Task<IEnumerable<Domain.Entities.EquipmentStatus>> Handle(GetAllEquipmentStatusesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}