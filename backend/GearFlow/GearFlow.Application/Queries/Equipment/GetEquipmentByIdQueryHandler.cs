using MediatR;
 using GearFlow.Domain.Entities;
 using GearFlow.Domain.Interfaces;
 
 public class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, Equipment?>

 {
     private readonly IEquipmentRepository _equipmentRepository;
 
     public GetEquipmentByIdQueryHandler(IEquipmentRepository equipmentRepository)
     {
         _equipmentRepository = equipmentRepository;
     }
 
     public Task<Equipment?> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
     {
         return _equipmentRepository.GetByIdAsync(request.Id);
     }
 }