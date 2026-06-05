using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.EquipmentType;

public class ArchiveEquipmentTypeCommandHandle : IRequestHandler<ArchiveEquipmentTypeCommand, bool>
{
    private readonly IRepository<Domain.Entities.EquipmentType> _repository;

    public ArchiveEquipmentTypeCommandHandle(IRepository<Domain.Entities.EquipmentType> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ArchiveEquipmentTypeCommand request, CancellationToken cancellationToken)
    {
        var type = await _repository.GetByIdAsync(request.Id);
        if (type == null)
        {
            throw new KeyNotFoundException("Equipment type not found");
        }

        if (type.Archived)
        {
            throw new InvalidOperationException("Equipment type already deleted");
        }

        type.Archived = true;

        await _repository.UpdateAsync(type);
        return true;
    }
}