using GearFlow.Application.DTOs;
using MediatR;

namespace GearFlow.Application.Queries.Auth;

public record GetCurrentUserQuery(int UserId) : IRequest<UserDto>;