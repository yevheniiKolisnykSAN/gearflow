using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Equipment;

public class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand, int>
{
    private readonly IRepository<Domain.Entities.Equipment> _equipmentRepository;

    public UpdateEquipmentCommandHandler(IRepository<Domain.Entities.Equipment> equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<int> Handle(UpdateEquipmentCommand query, CancellationToken cancellationToken)
    {

        var equipment = await _equipmentRepository.GetByIdAsync(query.Id);
        if (equipment == null) return 0;
        
        equipment.SerialNumber = query.SerialNumber;
        equipment.Location = query.Location;
        equipment.Specification = query.Specification;
        equipment.MaxLoanDays = query.MaxLoanDays;
        equipment.StatusId = query.StatusId;
        equipment.TypeId = query.TypeId;

        await _equipmentRepository.UpdateAsync(equipment);
        return equipment.Id;
    }
}