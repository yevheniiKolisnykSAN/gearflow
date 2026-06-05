using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Equipment;

public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, int>
{
    private readonly IRepository<Domain.Entities.Equipment> _equipmentRepository;

    public CreateEquipmentCommandHandler(IRepository<Domain.Entities.Equipment> equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<int> Handle(CreateEquipmentCommand query, CancellationToken cancellationToken)
    {
        var equipment = new Domain.Entities.Equipment
        {
            SerialNumber = query.SerialNumber,
            Name = query.Name,
            Specification = query.Specification,
            MaxLoanDays = query.MaxLoanDays,
            StatusId = query.StatusId,
            TypeId = query.TypeId,
            LocationId = query.LocationId
        };
        await _equipmentRepository.AddAsync(equipment);
        return equipment.Id;
    }
}