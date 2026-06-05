using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.EquipmentType;

public class UpdateEquipmentTypeCommandHandle : IRequestHandler<UpdateEquipmentTypeCommand, int>
{
    private readonly IRepository<Domain.Entities.EquipmentType> _repository;

    public UpdateEquipmentTypeCommandHandle(IRepository<Domain.Entities.EquipmentType> repository)
    {
        _repository = repository;
    }


    public async Task<int> Handle(UpdateEquipmentTypeCommand request, CancellationToken cancellationToken)
    {
        var type = await _repository.GetByIdAsync(request.Id);
        if (type == null) return 0;
        
        type.Name = request.Name;
        await _repository.UpdateAsync(type);
        return type.Id;
    }
}