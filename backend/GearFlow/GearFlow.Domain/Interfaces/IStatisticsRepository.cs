using GearFlow.Application.DTOs;

namespace GearFlow.Domain.Interfaces;

public interface IStatisticsRepository
{
    Task<StatisticsDto> GetStatisticsAsync();
}