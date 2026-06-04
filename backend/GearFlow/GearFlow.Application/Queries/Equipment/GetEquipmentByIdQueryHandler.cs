using MediatR;
 using GearFlow.Domain.Entities;
 using GearFlow.Domain.Interfaces;
 
 public class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, Equipment?>

 {
     private readonly IRepository<Equipment> _equipmentRepository;
 
     public GetEquipmentByIdQueryHandler(IRepository<Equipment> equipmentRepository)
     {
         _equipmentRepository = equipmentRepository;
     }
 
     public Task<Equipment?> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
     {
         return _equipmentRepository.GetByIdAsync(request.Id);
     }
 }