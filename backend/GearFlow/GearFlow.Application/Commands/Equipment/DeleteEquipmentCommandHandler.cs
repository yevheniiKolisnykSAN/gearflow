using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Equipment;

public class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand, bool>
{
    private readonly IRepository<Domain.Entities.Equipment> _equipmentRepository;

    public DeleteEquipmentCommandHandler(IRepository<Domain.Entities.Equipment> equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<bool> Handle(DeleteEquipmentCommand query, CancellationToken cancellationToken)
    {
        return await _equipmentRepository.DeleteAsync(query.Id);
    }
}