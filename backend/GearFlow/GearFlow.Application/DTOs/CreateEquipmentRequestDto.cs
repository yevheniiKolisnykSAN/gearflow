namespace GearFlow.Application.DTOs;

public record CreateEquipmentRequestDto(
    string Name,
    string Specification,
    int MaxLoanDays,
    int StatusId,
    int TypeId,
    int LocationId
);