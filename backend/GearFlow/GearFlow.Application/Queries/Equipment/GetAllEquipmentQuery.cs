using MediatR;
using GearFlow.Domain.Entities;
using GearFlow.Domain.Models;

public record GetAllEquipmentQuery(
    string? Search = null,
    string? Name = null,
    string? SerialNumber = null,
    string? Specification = null,
    int[]? MaxLoanDays = null,
    int[]? LocationId = null,
    int[]? StatusId = null,
    int[]? TypeId = null) : IRequest<IEnumerable<Equipment>>;