using GearFlow.Application.DTOs;
using MediatR;

namespace GearFlow.Application.Commands.Auth;

public record GetOrCreateMicrosoftUserCommand(string Email, string FirstName, string LastName) : IRequest<UserDto>;