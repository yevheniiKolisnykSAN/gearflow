namespace GearFlow.Application.DTOs;

public record UpdateEquipmentRequestDto(
    string Name,
    string Specification,
    int MaxLoanDays,
    int StatusId,
    int TypeId,
    int LocationId
);