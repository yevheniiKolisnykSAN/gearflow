using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Equipment;

public class ArchiveEquipmentCommandHandler : IRequestHandler<ArchiveEquipmentCommand, bool>
{
    private readonly IRepository<Domain.Entities.Equipment> _equipmentRepository;

    public ArchiveEquipmentCommandHandler(IRepository<Domain.Entities.Equipment> equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<bool> Handle(ArchiveEquipmentCommand query, CancellationToken cancellationToken)
    {
        var type = await _equipmentRepository.GetByIdAsync(query.Id);
        if (type == null)
        {
            throw new KeyNotFoundException("Equipment not found");
        }

        if (type.Archived)
        {
            throw new InvalidOperationException("Equipment already deleted");
        }

        type.Archived = true;

        await _equipmentRepository.UpdateAsync(type);
        return true;
    }
}