using GearFlow.Application.DTOs;
using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Queries.Statistic;

public class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, StatisticsDto>
{
    private readonly IStatisticsRepository _repository;

    public GetStatisticsQueryHandler(IStatisticsRepository repository)
    {
        _repository = repository;
    }

    public async Task<StatisticsDto> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetStatisticsAsync();
    }
}