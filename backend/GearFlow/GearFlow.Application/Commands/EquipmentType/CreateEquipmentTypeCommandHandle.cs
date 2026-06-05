using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.EquipmentType;

public class CreateEquipmentTypeCommandHandle : IRequestHandler<CreateEquipmentTypeCommand, int>
{
    private readonly IRepository<Domain.Entities.EquipmentType> _repository;

    public CreateEquipmentTypeCommandHandle(IRepository<Domain.Entities.EquipmentType> repository)
    {
        _repository = repository;
    }


    public async Task<int> Handle(CreateEquipmentTypeCommand request, CancellationToken cancellationToken)
    {
        var type = new Domain.Entities.EquipmentType
        {
            Name = request.Name
        };

        await _repository.AddAsync(type);
        return type.Id;
    }
}