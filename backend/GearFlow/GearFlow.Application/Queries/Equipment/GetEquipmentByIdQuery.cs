using MediatR;
using GearFlow.Domain.Entities;

public record GetEquipmentByIdQuery(int Id) : IRequest<Equipment?>;
