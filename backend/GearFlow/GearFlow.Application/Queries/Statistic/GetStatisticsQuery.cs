using GearFlow.Application.DTOs;
using MediatR;

namespace GearFlow.Application.Queries.Statistic;

public record GetStatisticsQuery() : IRequest<StatisticsDto>;