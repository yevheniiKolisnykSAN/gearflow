using GearFlow.Domain.Entities;
using GearFlow.Domain.Interfaces;
using MediatR;


public class GetAllEquipmentQueryHandler : IRequestHandler<GetAllEquipmentQuery, IEnumerable<Equipment>>
{
    private readonly IEquipmentRepository _equipmentRepository;

    public GetAllEquipmentQueryHandler(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<IEnumerable<Equipment>> Handle(GetAllEquipmentQuery query, CancellationToken cancellationToken)
    {
        return await _equipmentRepository.GetEquipmentList(
            query.Search,
            query.Name,
            query.SerialNumber,
            query.Specification,
            query.MaxLoanDays,
            query.LocationId,
            query.StatusId,
            query.TypeId);
    }
}