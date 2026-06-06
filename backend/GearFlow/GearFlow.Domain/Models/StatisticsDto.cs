namespace GearFlow.Application.DTOs;

public record StatisticsDto(
    int TotalReservations,
    int ActiveReservations,
    double AvgDurationDays,
    int TotalDefects,
    IEnumerable<TopEquipmentDto> TopEquipment,
    IEnumerable<MonthStatDto> ByMonth,
    IEnumerable<TypeStatDto> ByType);
    
public record TopEquipmentDto(string Name, int Count);
public record MonthStatDto(string Month, int Count);
public record TypeStatDto(string TypeName, int Count);