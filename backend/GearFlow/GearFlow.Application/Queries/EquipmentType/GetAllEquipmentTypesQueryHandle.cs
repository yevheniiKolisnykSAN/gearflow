using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.EquipmentType;

public class GetAllEquipmentTypesQueryHandle : IRequestHandler<GetAllEquipmentTypesQuery, IEnumerable<Domain.Entities.EquipmentType>>
{
    private readonly IRepository<Domain.Entities.EquipmentType> _repository;

    public GetAllEquipmentTypesQueryHandle(IRepository<Domain.Entities.EquipmentType> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Domain.Entities.EquipmentType>> Handle(GetAllEquipmentTypesQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(t => !t.Archived);
    }
}