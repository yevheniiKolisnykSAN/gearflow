using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using MediatR;


public class GetAllEquipmentQueryHandler : IRequestHandler<GetAllEquipmentQuery, IEnumerable<Equipment>>
{
    private readonly IRepository<Equipment> _equipmentRepository;

    public GetAllEquipmentQueryHandler(IRepository<Equipment> equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<IEnumerable<Equipment>> Handle(GetAllEquipmentQuery query, CancellationToken cancellationToken)
    {
        return await _equipmentRepository.GetAllAsync();
    }
}